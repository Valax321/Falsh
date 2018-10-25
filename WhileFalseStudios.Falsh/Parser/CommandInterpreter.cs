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

            List<string> commandArgs = new List<string>();
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "|") //Command pipelines
                {
                    if (commandArgs.Count == 0)
                    {
                        throw new ParseException(typeof(CommandToken), "Command must have 1 or more arguments");
                    }
                    List<string> toks = commandArgs.ToList(); //HACK for list copy
                    parsedTokens.Add(new CommandToken(toks));
                    commandArgs.Clear();
                }
                else if (tokens[i] == ">") // Out pipes
                {
                    if (tokens.Count - 1 == i + 1)
                    {
                        //This needs to go first so that the command is placed BEFORE the out pipe
                        List<string> toks = commandArgs.ToList(); //HACK for list copy
                        parsedTokens.Add(new CommandToken(toks));
                        commandArgs.Clear();

                        parsedTokens.Add(new OutPipeToken(tokens[i + 1]));
                        i++; //Skips the next token since we already pulled it in as part of the outpipe (the file.txt bit)
                    }
                    else if (tokens.Count - 1 < i + 1) //If we have too few tokens to properly parse
                    {
                        throw new ParseException(typeof(OutPipeToken), "Output pipe must provide a file to write stdout to");
                    }
                    else // We have too many tokens after this. It is not going to be the last token to appear in the expression
                    {
                        throw new ParseException(typeof(OutPipeToken), "Output pipe may only appear as the last expression in a command");
                    }
                }
                else //Normal command, add to arglist.
                {
                    commandArgs.Add(tokens[i]);
                    continue;
                }                
            }

            // Adds any remaining contents of the arglist to its own command token, without this the last command in the expression would be lost.
            if (commandArgs.Count > 0)
            {
                List<string> toks = commandArgs.ToList(); //HACK for list copy
                parsedTokens.Add(new CommandToken(toks));
                commandArgs.Clear();
            }

            //We no longer need to check if the last token is an OutPipeToken manually, as the code above will check as it parses the OutPipeToken.
            ParsedTokens = parsedTokens;
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
