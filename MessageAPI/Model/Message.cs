using System;
using System.Text;

namespace MessageAPI.Model
{
    [Serializable]
    public class MessageClass
    {
        public MessageClass()
        {
        }
        
        public MessageClass(string subject, string message, int senderId, int receiverId)
        {
            Subject = subject;
            Message = message;
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        /// <summary>
        /// Subject of message.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Message body.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// SenderId - user's position in list (zero based).
        /// </summary>
        public int SenderId { get; private set; }

        /// <summary>
        /// ReceiverId - user's position in list (zero based).
        /// </summary>
        public int ReceiverId { get; private set; }

        /// <summary>
        /// Generating random message body.
        /// </summary>
        /// <param name="alphabet">Input alphabet.</param>
        /// <param name="length">Length of message.</param>
        /// <returns>Random message with parameters.</returns>
        public static string GenRandomString(string alphabet, int length)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder(length - 1);
            
            int charIndex;
            for (int i = 0; i < length; i++)
            {
                charIndex = random.Next(0, alphabet.Length - 1);
                sb.Append(alphabet[charIndex]);
            }

            return sb.ToString();
        }
    }
}
