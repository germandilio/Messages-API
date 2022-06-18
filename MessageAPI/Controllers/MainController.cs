using System;
using System.Collections.Generic;
using System.Linq;
using MessageAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace MessageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : Controller
    {
        /// <summary>
        /// List of users.
        /// </summary>
        private List<User> _users;

        /// <summary>
        /// List of messages.
        /// </summary>
        private List<MessageClass> _messages;

        private readonly DataProcessing _dataProcessor;

        public MainController()
        {
            _dataProcessor = new DataProcessing();
        }

        /// <summary>
        /// Generating random messages and users.
        /// </summary>
        /// <returns>Http Action Result.</returns>
        [HttpPost("random")]
        public IActionResult RandomGeneration()
        {
            try
            {
                _dataProcessor.RunGeneration(out _users, out _messages);

                _users = _users.OrderBy(user => user.Email).ToList();
                _dataProcessor.Serialize(_users, _messages);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error in creating .json files. {ex.Message}");
            }

            return Ok("Success");
        }

        /// <summary>
        /// Loading data from json file.
        /// </summary>
        /// <returns>Http Action Result.</returns>
        [HttpGet("loadDataFromJson")]
        public IActionResult LoadData()
        {
            // Verification that files exists.
            if (!System.IO.File.Exists("Saved/Users.json"))
                return NotFound("File not found.");
            if (!System.IO.File.Exists("Saved/Messages.json"))
                return NotFound("File not found.");

            try
            {
                _dataProcessor.Deserialize(out _users, out _messages);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error in reading .json files. {ex.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// Saving data to json file.
        /// </summary>
        /// <returns>Http Action Result.</returns>
        [HttpGet("saveDataToJson")]
        public IActionResult SaveData()
        {
            try
            {
                _dataProcessor.Serialize(_users, _messages);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error in serialization: {ex.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>Http Action Result with list of users.</returns>
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            if (_users == null)
                return NotFound("Users list is null or empty.");

            return Ok(_users);
        }

        /// <summary>
        /// Get list of users with specified amount limit and offset from beginning.
        /// </summary>
        /// <param name="limit">Max amount of users.</param>
        /// <param name="offset">The number of users to skip from start.</param>
        /// <returns>Http Action Result with list of users.</returns>
        [HttpGet("users/{limit}&{offset}")]
        public IActionResult GetUsers(int limit, int offset)
        {
            if (_users == null)
                return NotFound("Users list is null or empty.");

            // Parameters verification.
            if (limit <= 0 || offset < 0)
                return NotFound("Offset can't be <0 and limit can't be <= 0.");


            var users = _users.Skip(offset).Take(limit);

            // If request body is empty.
            if (users.Any())
                return NotFound("Data with this parameters doesn't exist.");

            return Ok(users);
        }

        /// <summary>
        /// Get user by email.
        /// </summary>
        /// <param name="email">User's parameter.</param>
        /// <returns>Http Action Result with list of users.</returns>
        [HttpGet("users/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            if (email == null)
                return NotFound("Users by email not found.");

            User user = _users.FirstOrDefault(x => x.Email == email);
            if (user == null)
                return NotFound("Users by email not found.");

            return Ok(user);
        }

        /// <summary>
        /// Get messages by sender ID.
        /// </summary>
        /// <param name="senderId">User's position in list (zero based).</param>
        /// <returns>Http Action Result with list of messages.</returns>
        [HttpGet("messages/{senderId}")]
        public IActionResult GetMessagesBySender(int senderId)
        {
            if (_messages == null)
                return NotFound("Messages not found.");

            var foundMessages = Select(_messages, ms => ms.SenderId == senderId);
            if (foundMessages.Any())
                return NotFound("Messages not found.");

            return Ok(foundMessages);
        }

        /// <summary>
        /// Get messages by receiver ID.
        /// </summary>
        /// <param name="receiverId">User's position in list (zero based).</param>
        /// <returns>Http Action Result with list of messages.</returns>
        [HttpGet("{receiverId}/messages")]
        public IActionResult GetMessagesByReceiver(int receiverId)
        {
            if (_messages == null)
                return NotFound("Messages not found.");

            var messages = Select(_messages, ms => ms.ReceiverId == receiverId);
            if (messages.Any())
                return NotFound("Messages not found.");

            return Ok(messages);
        }

        /// <summary>
        /// Get messages by senderId and receiverId.
        /// </summary>
        /// <param name="senderId">User's position in list (zero based).</param>
        /// <param name="receiverId">User's position in list (zero based).</param>
        /// <returns>Http Action Result with list of messages.</returns>
        [HttpGet("{receiverId}/messages/{senderId}")]
        public IActionResult GetMessages(int senderId, int receiverId)
        {
            if (_messages == null)
                return NotFound("Messages not found.");
            
            var messages = Select(_messages, ms => ms.ReceiverId == receiverId && ms.SenderId == senderId);
            if (messages.Any())
                return NotFound("Messages not found.");

            return Ok(messages);
        }

        private IEnumerable<T> Select<T>(IEnumerable<T> collection, Func<T, bool> filter)
        {
            if (collection == null)
                return new List<T>();
            if (filter == null)
                return collection;
            
            return collection.Where(filter);
        }

        /// <summary>
        /// Add user to list.
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="email">User's email</param>
        /// <returns>Http Action Result.</returns>
        [HttpPost("users/add/{userName}&{email}")]
        public IActionResult AddUser(string userName, string email)
        {
            if (userName == null || email == null)
                return BadRequest();

            _users ??= new List<User>();
            _users.Add(new User(userName, email, _users.Count));
            
            return Ok("User was successfully added.");
        }

        /// <summary>
        /// Add message to list.
        /// </summary>
        /// <param name="subject">Subject of message</param>
        /// <param name="message">Body</param>
        /// <param name="senderId">From {senderId}</param>
        /// <param name="receiverId">To {receiverId}</param>
        /// <returns>Http Action Result.</returns>
        [HttpPost("messages/add/{subject}&{message}&{senderId}&{receiverId}")]
        public IActionResult AddMessage(string subject, string message, int senderId, int receiverId)
        {
            if (subject == null || message == null)
                return BadRequest();

            // Check if sender and receiver are exists.
            if (!_users.Exists(user => user.Id == senderId))
                return BadRequest(senderId);
            if (!_users.Exists(user => user.Id == receiverId))
                return BadRequest(receiverId);


            _users ??= new List<User>();
            _messages.Add(new MessageClass(subject, message, senderId, receiverId));
            
            return Ok("Message was successfully added.");
        }
    }
}