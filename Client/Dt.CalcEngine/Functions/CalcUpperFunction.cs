#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Globalization;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Converts text to uppercase.
    /// </summary>
    public class CalcUpperFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Converts text to uppercase.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: text.
        /// </para>
        /// <para>
        /// Text is the text you want converted to uppercase.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return CalcConvert.ToString(args[0]).ToUpper(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "UPPER";
            }
        }
    }
}

