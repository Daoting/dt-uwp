#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Specifies the unsupport record category
    /// </summary>
    public enum RecordCategory
    {
        Drawing,
        DrawingGroup,
        TextRun,
        FontX,
        Formula,
        DrawingFileRelationShip,
        DrawingFileRelationFile,
        SheetFileRelationShip,
        SheetFileRelationFile,
        VBA,
        LegacyDrawing,
        AlternateContent,
        VmlMediaFile,
        VmlMediaFileRelationShip
    }
}

