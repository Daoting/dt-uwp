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
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the most frequently occurring, or repetitive, value in
    /// an array or range of data.
    /// </summary>
    public class CalcModeFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Returns the most frequently occurring, or repetitive, value in
        /// an array or range of data.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: number1, [number2], [number3], ..
        /// </para>
        /// <para>
        /// Number1, number2, ... are 1 to 255 arguments for which you want to calculate the mode.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj2 = null;
            int num = 0;
            List<double> list = new List<double>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int k = 0; k < ArrayHelper.GetLength(args[i], 0); k++)
                    {
                        object obj3 = ArrayHelper.GetValue(args[i], k, 0);
                        if (CalcConvert.IsNumber(obj3))
                        {
                            double item = CalcConvert.ToDouble(obj3);
                            list.Add(item);
                        }
                        else if (obj3 is CalcError)
                        {
                            return obj3;
                        }
                    }
                }
                else
                {
                    double num5;
                    if (!CalcConvert.TryToDouble(args[i], out num5, true))
                    {
                        return CalcErrors.Value;
                    }
                    list.Add(num5);
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                int num7 = 0;
                for (int m = 0; m < list.Count; m++)
                {
                    if ((m != j) && (((double) list[m]) == ((double) list[j])))
                    {
                        num7++;
                    }
                }
                if (num7 > num)
                {
                    num = num7;
                    obj2 = (double) list[j];
                }
            }
            if (num == 0)
            {
                return CalcErrors.NotAvailable;
            }
            return obj2;
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        public override int MaxArgs
        {
            get
            {
                return 0xff;
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
                return "MODE";
            }
        }
    }
}

