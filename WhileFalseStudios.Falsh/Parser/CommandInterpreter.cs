using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    class CommandInterpreter
    {
        private Tokeniser m_tokeniser;
        public List<IParsedToken> ParsedTokens { get; private set; }

        public CommandInterpreter(string cmd)
        {
            m_tokeniser = new Tokeniser(cmd);
        }

        /// <summary>
        /// Parses the input command into executable tokens.
        /// </summary>
        /// <exception cref="ParseException"></exception>
        public void Parse()
        {
            List<IParsedToken> parsedTokens = new List<IParsedToken>();

            var tokens = m_tokeniser.GetTokens();
            if (tokens.Count > 1 && tokens[1] == ">")
            {
                string inFile = tokens[0];
                tokens.RemoveRange(0, 2); //Remove the 'file > ' section from the statement
                parsedTokens.Add(new InPipeToken(inFile));
            }

            List<string> commandArgs = new List<string>();
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "|")
                {
                    if (commandArgs.Count == 0)
                    {
                        throw new ParseException(typeof(CommandToken), "Command must have 1 or more arguments");
                    }
                    List<string> toks = commandArgs.ToList(); //HACK for list copy
                    parsedTokens.Add(new CommandToken(toks));
                    commandArgs.Clear();
                }
                else if (tokens[i] == ">")
                {
                    if (tokens.Count - 1 == i + 1)
                    {
                        parsedTokens.Add(new OutPipeToken(tokens[i + 1]));

                        List<string> toks = commandArgs.ToList(); //HACK for list copy
                        parsedTokens.Add(new CommandToken(toks));
                        commandArgs.Clear();
                    }
                    else
                    {
                        throw new ParseException(typeof(OutPipeToken), "Output pipe must provide a file to write stdout to");
                    }
                }
                else
                {
                    commandArgs.Add(tokens[i]);
                    continue;
                }                
            }

            if (commandArgs.Count > 0)
            {
                List<string> toks = commandArgs.ToList(); //HACK for list copy
                parsedTokens.Add(new CommandToken(toks));
                commandArgs.Clear();
            }

            var index = from token in parsedTokens where token is OutPipeToken select parsedTokens.IndexOf(token); //Checking if the last token only is a OutPipeToken
            if (index.Count() == 0 || (index.Count() == 1 && index.First() == parsedTokens.Count - 1)) ParsedTokens = parsedTokens;
            else throw new ParseException(parsedTokens[index.First()].GetType(), "Output pipe may only appear as the last expression in a command");
        }
    }

    #region Parser Types
    class ParseException : Exception
    {
        private Type m_tokenType;
        private string m_message;

        public ParseException(Type t, string msg)
        {
            m_tokenType = t;
            m_message = msg;
        }

        public override string Message => $"({m_tokenType.Name}) {m_message}";
    }

    interface IParsedToken
    {
        string Value { get; }
        void Execute(ShellContext ctx);
    }
    #endregion
}
