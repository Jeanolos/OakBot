﻿using System;
using System.Text.RegularExpressions;

namespace OakBot
{
    public enum uType
    {
        BOT,
        VIEWER,
        MODERATOR,
        GLOBALMODERATOR,
        ADMIN,
        STAFF
    }

    public class IrcMessage
    {
        #region Private Fields

        private string[] arguments;

        // Basic IRC Message
        private string author;

        private string command;

        private string displayName;

        private string emotes;

        private string host;

        private string message;

        private TwitchCredentials messageSource;

        private bool moderator;

        // IRC v3 TAGS
        private string nameColor;

        //private string roomId;
        private bool subscriber;

        // Tracking
        private DateTime timestamp;

        private bool turbo;
        private int userId;
        private uType userType;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Constructor to be used to parse receoved IRC message.
        /// </summary>
        /// <param name="receivedLine">Received IRC message.</param>
        /// <param name="loggedinAccount">Account that is logged in to the IRC.</param>
        public IrcMessage(string receivedLine, TwitchCredentials loggedinAccount)
        {
            this.timestamp = DateTime.Now;
            this.messageSource = loggedinAccount;

            // First get all arguments if starts with @
            if (receivedLine.StartsWith("@"))
            {
                MatchCollection ircTags = Regex.Matches(receivedLine, @"(?<arg>[\w-]+)=(?<value>[\w:#,-\/]*);?");
                foreach (Match m in ircTags)
                {
                    switch (m.Groups["arg"].Value)
                    {
                        case "color":
                            nameColor = m.Groups["value"].Value;
                            break;

                        case "display-name":
                            displayName = m.Groups["value"].Value;
                            break;

                        case "emotes":
                            emotes = m.Groups["value"].Value;
                            break;

                        case "mod":
                            moderator = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "subscriber":
                            subscriber = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "turbo":
                            turbo = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "user-id":
                            userId = int.Parse(m.Groups["value"].Value);
                            break;

                        case "user-type":
                            switch (m.Groups["value"].Value)
                            {
                                case "mod":
                                    userType = uType.MODERATOR;
                                    break;

                                case "global_mod":
                                    userType = uType.GLOBALMODERATOR;
                                    break;

                                case "admin":
                                    userType = uType.ADMIN;
                                    break;

                                case "staff":
                                    userType = uType.STAFF;
                                    break;

                                default:
                                    userType = uType.VIEWER;
                                    break;
                            }
                            break;
                    }
                }
            }

            // Get the base IRC message
            //Match ircMessage = Regex.Match(receivedLine, @"(?<!\S)(?::(?:(?<author>\w+)!)?(?<host>\S+) )?(?<command>\w+)(?: (?!:)(?<args>.+?))?(?: :(?<message>.+))?$");
            Match ircMessage = Regex.Match(receivedLine,
                @"(?<!\S)(?::(?:(?<author>\w+)!)?(?<host>\S+) )?(?<command>\w+)(?: (?<args>.+?))?(?: :(?<message>.+))?$");

            author = ircMessage.Groups["author"].Value;
            host = ircMessage.Groups["host"].Value;
            command = ircMessage.Groups["command"].Value;
            arguments = ircMessage.Groups["args"].Value.Split(' ');
            message = ircMessage.Groups["message"].Value;
        }

        /// <summary>
        /// Constructor to use to create manually an chat message.
        /// </summary>
        /// <param name="author">Author of the message.</param>
        /// <param name="message">Message content.</param>
        public IrcMessage(string author, string message)
        {
            this.timestamp = DateTime.Now;
            this.message = message;
            this.author = author;
            this.userType = uType.BOT;
        }

        #endregion Public Constructors

        #region Public Properties

        public string[] Args
        {
            get
            {
                return arguments;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }
        }

        public string Command
        {
            get
            {
                return command;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(displayName) ? author : displayName;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        public string NameColor
        {
            get
            {
                return string.IsNullOrEmpty(nameColor) ? "#2E8B57" : nameColor;
            }
        }

        public string ShortTime
        {
            get
            {
                return timestamp.ToShortTimeString();
            }
        }

        public uType UserType
        {
            get
            {
                return userType;
            }
        }

        #endregion Public Properties
    }
}