using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace WhileFalseStudios.Falsh
{
    public class Tokeniser
    {
        protected string m_inputString;

        public Tokeniser(string input)
        {
            m_inputString = input;
        }

        public virtual List<string> GetTokens()
        {
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool parseLiteral = false;
            bool escapeNextChar = false;
            foreach (char c in m_inputString)
            {
                if (escapeNextChar)
                {
                    sb.Append(c);
                    escapeNextChar = false;
                    continue;
                }

                if (c == '"')
                {
                    parseLiteral = !parseLiteral;
                    //sb.Append(c);
                    continue;
                }

                if (c == '\\')
                {
                    escapeNextChar = true;
                    continue;
                }

                if (!parseLiteral)
                {
                    if (c == ' ')
                    {
                        if (sb.Length > 0)
                        {
                            tokens.Add(sb.ToString());
                            sb.Clear();
                        }                        
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    continue;
                }
                else
                {
                    sb.Append(c);
                    continue;
                }
            }

            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }

            return tokens;
        }
    }
}
