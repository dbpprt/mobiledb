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

namespace MobileDB.Common.ExpressiveAnnotations
{
    /// <summary>
    ///     Token identifier.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        ///     Logical conjunction.
        /// </summary>
        AND,

        /// <summary>
        ///     Logical disjunction.
        /// </summary>
        OR,

        /// <summary>
        ///     Logical negation.
        /// </summary>
        NOT,

        /// <summary>
        ///     Greater than or equal to.
        /// </summary>
        GE,

        /// <summary>
        ///     Less than or equal to.
        /// </summary>
        LE,

        /// <summary>
        ///     Greater than.
        /// </summary>
        GT,

        /// <summary>
        ///     Less than.
        /// </summary>
        LT,

        /// <summary>
        ///     Equal to.
        /// </summary>
        EQ,

        /// <summary>
        ///     Not equal to.
        /// </summary>
        NEQ,

        /// <summary>
        ///     Addition.
        /// </summary>
        ADD,

        /// <summary>
        ///     Subtraction.
        /// </summary>
        SUB,

        /// <summary>
        ///     Multiplication.
        /// </summary>
        MUL,

        /// <summary>
        ///     Division.
        /// </summary>
        DIV,

        /// <summary>
        ///     Left bracket.
        /// </summary>
        LEFT_BRACKET,

        /// <summary>
        ///     Right bracket.
        /// </summary>
        RIGHT_BRACKET,

        /// <summary>
        ///     Comma.
        /// </summary>
        COMMA,

        /// <summary>
        ///     NULL.
        /// </summary>
        NULL,

        /// <summary>
        ///     Integer value.
        /// </summary>
        INT,

        /// <summary>
        ///     Boolean value.
        /// </summary>
        BOOL,

        /// <summary>
        ///     Float value.
        /// </summary>
        FLOAT,

        /// <summary>
        ///     String.
        /// </summary>
        STRING,

        /// <summary>
        ///     Function.
        /// </summary>
        FUNC,

        /// <summary>
        ///     EOF.
        /// </summary>
        EOF
    }
}
