#region Copyright (C) 2014 Dennis Bappert
// The MIT License (MIT)

// Copyright (c) 2014 Dennis Bappert

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MobileDB.Common.ExpressiveAnnotations
{
    /// <summary>
    ///     Performs lexical analysis of provided logical expression.
    /// </summary>
    public sealed class Lexer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Lexer" /> class.
        /// </summary>
        public Lexer()
        {
            // special characters (should be escaped if needed): .$^{[(|)*+?\
            RegexMap = new Dictionary<TokenType, string>
            {
                {TokenType.AND, @"&&"},
                {TokenType.OR, @"\|\|"},
                {TokenType.LEFT_BRACKET, @"\("},
                {TokenType.RIGHT_BRACKET, @"\)"},
                {TokenType.GE, @">="},
                {TokenType.LE, @"<="},
                {TokenType.GT, @">"},
                {TokenType.LT, @"<"},
                {TokenType.EQ, @"=="},
                {TokenType.NEQ, @"!="},
                {TokenType.NOT, @"!"},
                {TokenType.NULL, @"null"},
                {TokenType.COMMA, @","},
                {TokenType.FLOAT, @"[\+-]?\d*\.\d+(?:[eE][\+-]?\d+)?"},
                {TokenType.INT, @"[\+-]?\d+"},
                {TokenType.ADD, @"\+"},
                {TokenType.SUB, @"-"},
                {TokenType.MUL, @"\*"},
                {TokenType.DIV, @"/"},
                {TokenType.BOOL, @"(?:true|false)"},
                {TokenType.STRING, @"([""'])(?:\\\1|.)*?\1"},
                {TokenType.FUNC, @"[a-zA-Z_]+(?:(?:\.[a-zA-Z_])?[a-zA-Z0-9_]*)*"}
            };
        }

        private Token Token { get; set; }
        private string Expression { get; set; }
        private IDictionary<TokenType, string> RegexMap { get; set; }
        private IDictionary<TokenType, Regex> CompiledRegexMap { get; set; }

        /// <summary>
        ///     Analyzes the specified logical expression and extracts the array of tokens.
        /// </summary>
        /// <param name="expression">The logical expression.</param>
        /// <returns>
        ///     Array of extracted tokens.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">expression;Expression not provided.</exception>
        public IEnumerable<Token> Analyze(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression", "Expression not provided.");

            Expression = expression;
            CompiledRegexMap = RegexMap.ToDictionary(kvp => kvp.Key, kvp => new Regex(string.Format("^{0}", kvp.Value)));

            var tokens = new List<Token>();
            while (Next())
                tokens.Add(Token);

            // once we've reached the end of the string, EOF token is returned - thus, parser's lookahead does not have to worry about running out of tokens
            tokens.Add(new Token(TokenType.EOF, string.Empty));

            return tokens;
        }

        private bool Next()
        {
            Expression = Expression.Trim();
            if (string.IsNullOrEmpty(Expression))
                return false;

            foreach (var kvp in CompiledRegexMap)
            {
                var regex = kvp.Value;
                var match = regex.Match(Expression);
                var value = match.Value;
                if (value.Length > 0)
                {
                    Token = new Token(kvp.Key, ConvertTokenValue(kvp.Key, value));
                    Expression = Expression.Substring(value.Length);
                    return true;
                }
            }
            throw new InvalidOperationException(string.Format("Invalid token started at: {0}", Expression));
        }

        private object ConvertTokenValue(TokenType type, string value)
        {
            switch (type)
            {
                case TokenType.NULL:
                    return null;
                case TokenType.INT:
                    return int.Parse(value);
                case TokenType.FLOAT:
                    return double.Parse(value);
                    // by default, treat real numeric literals as 64-bit floating binary point values (as C# does, gives better precision than float)
                case TokenType.BOOL:
                    return bool.Parse(value);
                case TokenType.STRING:
                    return value.Substring(1, value.Length - 2);
                default:
                    return value;
            }
        }
    }
}
