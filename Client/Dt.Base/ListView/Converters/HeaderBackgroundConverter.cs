﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// IsSelected -> 背景
    /// </summary>
    class HeaderBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Res.中黄 : Res.浅灰1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}