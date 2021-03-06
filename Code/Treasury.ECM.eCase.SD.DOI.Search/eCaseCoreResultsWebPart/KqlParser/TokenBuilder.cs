using System.Collections.Generic;
using System.Linq;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.KqlParser
{
    /// <summary>
    /// Parses a kql query string into kql tokens
    /// This source code is released under the MIT license
    /// </summary>
    public class TokenBuilder
    {
        private readonly List<Token> _andExpr = new List<Token>();
        private readonly List<Token> _orExpr = new List<Token>();
        private readonly List<Token> _notExpr = new List<Token>();
        private readonly string _query;
        private TokenType _allowed;
        public TokenBuilder(string query, TokenType allowedTypes)
        {
            _query = query;
            _allowed = allowedTypes;
        }

        public List<Token> AndExpr
        {
            get { return _andExpr; }
        }

        public List<Token> OrExpr
        {
            get { return _orExpr; }
        }

        public List<Token> NotExpr
        {
            get { return _notExpr; }
        }

        public void Build()
        {
            TokenParser tp = new TokenParser(_query);
            IEnumerator<Token> enumerator = tp.GetEnumerator();
            Stack<Token> tokens = new Stack<Token>();
            Stack<TokenType> allowedStack = new Stack<TokenType>();
            while (enumerator.MoveNext())
            {
                if (allowedStack.Count > 0)
                    _allowed = allowedStack.Pop();
                Token token = enumerator.Current;
                TokenType type = token.Type;
                if ((_allowed & type) == type)
                {
                    switch (type)
                    {
                        case TokenType.Group:
                            break;
                        case TokenType.Operator:
                            var op = token.Text;
                            if (op == "OR")
                            {
                                if (tokens.Count > 0)
                                    _orExpr.Add(tokens.Pop());
                            }
                            else if (op == "AND")
                            {
                                if (tokens.Count > 0)
                                {
                                    _andExpr.Add(tokens.Pop());
                                }
                            }
                            allowedStack.Push(_allowed);
                            _allowed ^= TokenType.Operator;
                            allowedStack.Push(_allowed);
                            break;
                        case TokenType.Property:
                            var property = token;
                            enumerator.MoveNext();
                            token = enumerator.Current;
                            if (token == null) break;
                            if (token.Type != TokenType.Phrase && token.Type != TokenType.Word) continue;
                            property.Text += token.Text;
                            token = property;
                            break;
                        case TokenType.Phrase:
                        case TokenType.Word:
                            break;
                    }

                    bool addToStack = true;
                    while ( token != null && tokens.Count > 0)
                    {
                        var lastToken = tokens.Pop() ?? new Token();
                        if (lastToken.Type == TokenType.Operator && lastToken.Text == "AND")
                        {
                            token.ParentOperator = "AND";
                            _andExpr.Add(token);
                            addToStack = false;
                        }
                        else if (lastToken.Type == TokenType.Operator && lastToken.Text == "OR")
                        {
                            token.ParentOperator = "OR";
                            _orExpr.Add(token);
                            addToStack = false;
                        }
                        else if (lastToken.Type == TokenType.Operator && lastToken.Text == "NOT")
                        {
                            token.ParentOperator = "NOT";
                            _notExpr.Add(token);
                            addToStack = false;
                        }
                        else if (lastToken.Type == TokenType.Operator && lastToken.Text == "ANY")
                        {
                            token.ParentOperator = "ANY";
                            _orExpr.Add(token);
                            addToStack = false;
                        }
                        else if (lastToken.Type == TokenType.Operator && lastToken.Text == "ALL")
                        {
                            token.ParentOperator = "ALL";
                            _andExpr.Add(token);
                            addToStack = false;
                        }
                        else if (lastToken.Type == TokenType.Operator && lastToken.Text == "NONE")
                        {
                            token.ParentOperator = "NONE";
                            _notExpr.Add(token);
                            addToStack = false;
                        }
                        else
                        {
                            if (lastToken.Text.StartsWith("-"))
                            {
                                lastToken.Text = lastToken.Text.Trim('-');
                                lastToken.ParentOperator = "NONE";
                                _notExpr.Add(lastToken);
                            }
                            else
                            {
                                lastToken.ParentOperator = "ALL";
                                _andExpr.Add(lastToken);
                            }
                        }
                    }
                    if (addToStack)
                        tokens.Push(token);
                }
            }
            while (tokens.Count > 0)
            {
                var lastToken = tokens.Pop();
                if (lastToken.Text.StartsWith("-"))
                {
                    lastToken.Text = lastToken.Text.Trim('-');
                    _notExpr.Add(lastToken);
                }
                else
                {
                    lastToken.ParentOperator = "ALL";
                    _andExpr.Add(lastToken);
                }
            }

            GroupEqualPropertyKeys();
        }

        /// <summary>
        /// Equal properties should be considered an or, unless specifically have AND between them
        /// </summary>
        private void GroupEqualPropertyKeys()
        {
            Dictionary<string, List<Token>> groupProperties = new Dictionary<string, List<Token>>();
            foreach (Token token in _andExpr.Where(t => t.Type == TokenType.Property))
            {
                string propertyName = token.Text.Substring(0, token.Text.IndexOfAny(new [] {':', '=', '>', '<'}));
                List<Token> properties;
                if (!groupProperties.TryGetValue(propertyName, out properties))
                {
                    groupProperties[propertyName] = properties = new List<Token>();
                }
                properties.Add(token);
            }

            foreach (var value in groupProperties.Values)
            {
                if (value.Count <= 1) continue;
                if (value.Any(token => token.ParentOperator == "AND")) continue;
                foreach (var token in value)
                {
                    _andExpr.Remove(token);
                }

                Token multiProp = new Token();
                string joinOperator = " OR ";
                multiProp.Text = string.Join(joinOperator, value.Select(t => t.Text).ToArray());
                multiProp.Type = TokenType.Group;
                multiProp.ParentOperator = "AND";
                _andExpr.Add(multiProp);
            }
        }
    }
}
