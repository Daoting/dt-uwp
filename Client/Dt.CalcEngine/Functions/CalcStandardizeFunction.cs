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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns a normalized value from a distribution characterized by mean and standard_dev.
    /// </summary>
    public class CalcStandardizeFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a normalized value from a distribution characterized by mean and standard_dev.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: x, mean, standard_dev.
        /// </para>
        /// <para>
        /// X is the value you want to normalize.
        /// </para>
        /// <para>
        /// Mean is the arithmetic mean of the distribution.
        /// </para>
        /// <para>
        /// Standard_dev is the standard deviation of the distribution.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (num3 <= 0.0)
            {
                return CalcErrors.Number;
            }
            return (double) ((num - num2) / num3);
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
                return 3;
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
                return 3;
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
                return "STANDARDIZE";
            }
        }
    }
}

