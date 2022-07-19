﻿using System;
using System.Text.RegularExpressions;
using Windows.UI;

namespace DrumMidiEditorApp.pGeneralFunction.pWinUI;

/// <summary>
/// 色変換補助
/// </summary>
public static class ColorHelper
{
    /// <summary>
    /// 空色
    /// </summary>
    public static Color EmptyColor { get; } = Color.FromArgb( 0, 0, 0, 0 );

    /// <summary>
    /// #FFFFFF 表記の色、または ARGB値 からColorを返す
    /// </summary>
    /// <param name="aValue">テキスト</param>
    /// <returns>Color</returns>
    public static Color GetColor( string aValue )
    {
        try
        {
            var colorText = aValue.Replace( "#", String.Empty ).ToUpper();

            if ( Regex.IsMatch( colorText, "^[0-9A-F]+$" ) )
            { 
                switch ( colorText.Length )
                {
                    case 3:
                        return Color.FromArgb
                            ( 
                                255, 
                                Convert.ToByte( colorText.Substring( 0, 1 ), 16 ),
                                Convert.ToByte( colorText.Substring( 1, 1 ), 16 ),
                                Convert.ToByte( colorText.Substring( 2, 1 ), 16 )
                            );
                    case 4:
                        return Color.FromArgb
                            ( 
                                Convert.ToByte( colorText.Substring( 0, 1 ), 16 ),
                                Convert.ToByte( colorText.Substring( 1, 1 ), 16 ),
                                Convert.ToByte( colorText.Substring( 2, 1 ), 16 ),
                                Convert.ToByte( colorText.Substring( 3, 1 ), 16 )
                            );
                    case 6:
                        return Color.FromArgb
                            ( 
                                255, 
                                Convert.ToByte( colorText.Substring( 0, 2 ), 16 ),
                                Convert.ToByte( colorText.Substring( 2, 2 ), 16 ),
                                Convert.ToByte( colorText.Substring( 4, 2 ), 16 )
                            );
                    case 8:
                        return Color.FromArgb
                            ( 
                                Convert.ToByte( colorText.Substring( 0, 2 ), 16 ),
                                Convert.ToByte( colorText.Substring( 2, 2 ), 16 ),
                                Convert.ToByte( colorText.Substring( 4, 2 ), 16 ),
                                Convert.ToByte( colorText.Substring( 6, 2 ), 16 )
                            );
                }
            }
        }
        catch ( Exception )
        {
        }
        throw new Exception( "Color conversion error" );
    }

    /// <summary>
    /// #FFFFFFFF 表記の文字を返す
    /// </summary>
    /// <param name="aColor">Color</param>
    /// <returns>#FFFFFF 表記の文字</returns>
    public static string GetColor( Color aColor )
        => ( $"#{aColor.A:X2}{aColor.R:X2}{aColor.G:X2}{aColor.B:X2}" ).ToUpper();
}
