using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MessageAPI.Model;

namespace MessageAPI
{
    public class DataProcessing
    {
        private const string SerializationUsersPath = "Saved/Users.json";
        private const string SerializationMessagesPath = "Saved/Messages.json";

        private const string Alphabet = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
        private const string EmailHost = "@edu.hse.ru";
        
        public DataProcessing()
        {
        }

        /// <summary>
        /// Generating random users and messages for testing.
        /// </summary>
        internal void RunGeneration(out List<User> users, out List<MessageClass> messages)
        {
            Random random = new Random();

            users = new List<User>();
            for (int i = 0; i < random.Next(1, 50); i++)
            {
                string name = "User" + i;
                users.Add(new Model.User(name, name + EmailHost, users.Count()));
            }

            messages = new List<MessageClass>();

            for (int i = 0; i < random.Next(1, 50); i++)
            {
                messages.Add(new MessageClass("Studying",
                    MessageClass.GenRandomString(Alphabet,
                    random.Next(1, 100)), users[random.Next(0, users.Count)].Id,
                    users[random.Next(0, users.Count)].Id));
            }
        }

        /// <summary>
        /// Serialization of data to json file.
        /// </summary>
        internal void Serialize(List<User> users, List<MessageClass> messages)
        {
            // Users.
            Serialize(SerializationUsersPath, FileMode.Create, users);

            // Messages.
            Serialize(SerializationMessagesPath, FileMode.Create, messages);
        }

        /// <summary>
        /// Deserialization data from json file.
        /// </summary>
        internal void Deserialize(out List<User> users, out List<MessageClass> messages)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // Users.
            users = Deserialize<List<User>>(SerializationUsersPath, jsonOptions);
            
            // Messages.
            messages = Deserialize<List<MessageClass>>(SerializationMessagesPath, jsonOptions);
        }

        private void Serialize<T>(string path, FileMode mode, List<T> collection)
        {
            using var stream = File.Open(path, mode);
            using var writer = new Utf8JsonWriter(stream);
            
            JsonSerializer.Serialize(writer, collection);
        }

        private T Deserialize<T>(string path, JsonSerializerOptions options)
        {
            using var jsonReader = new StreamReader(path);
            
            return JsonSerializer.Deserialize<T>(jsonReader.ReadToEnd(), options);
        }
    }
}
