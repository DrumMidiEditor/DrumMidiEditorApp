﻿using System;
using DrumMidiClassLibrary.pUtil;
using DrumMidiClassLibrary.pWinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using Windows.Foundation;
using Windows.Storage;

namespace DrumMidiEditorApp.pView.pPlayer.pSurface.pScoreType2;

/// <summary>
/// プレイヤー描画アイテム：ノート
/// </summary>
internal class DmsItemNote : DisposeBaseClass, IComparable, IComparable<DmsItemNote>
{
    /// <summary>
    /// 描画範囲
    /// </summary>
    private Rect _DrawRect = new();

    /// <summary>
    /// 描画書式
    /// </summary>
    private FormatRect? _FormatRect = null;

    /// <summary>
    /// ラベルテキスト
    /// </summary>
    private readonly string _LabelText = "";


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="aX">描画位置＋１小節内での相対X座標</param>
    /// <param name="aY">描画位置＋１小節内での相対Y座標</param>
    /// <param name="aWidth">横幅</param>
    /// <param name="aHeight">高さ</param>
    /// <param name="aFormatRect">描画書式</param>
    /// <param name="aLabelText">ラベル</param>
    public DmsItemNote( float aX, float aY, float aWidth, float aHeight, FormatRect aFormatRect, string aLabelText )
    {
        _DrawRect.X = aX;
        _DrawRect.Y = aY;
        _DrawRect.Width = aWidth;
        _DrawRect.Height = aHeight;
        _FormatRect = aFormatRect;
        _LabelText = aLabelText;
    }

    protected override void Dispose( bool aDisposing )
    {
        if ( !_Disposed )
        {
            if ( aDisposing )
            {
                // Dispose managed resources.
                _FormatRect = null;
            }

            // Dispose unmanaged resources.

            _Disposed = true;

            // Note disposing has been done.
            base.Dispose( aDisposing );
        }
    }
    private bool _Disposed = false;

    /// <summary>
    /// 描画
    /// </summary>
    /// <param name="aGraphics">グラフィック</param>
    /// <param name="aDiffX">描画差分X</param>
    /// <param name="aDiffY">描画差分Y</param>
    public void Draw( CanvasDrawingSession aGraphics, float aDiffX, float aDiffY )
    {
        if ( _FormatRect == null )
        {
            return;
        }

        var rect = _DrawRect;
        rect.X += aDiffX;
        rect.Y += aDiffY;

        // テキスト
        if ( _LabelText.Length != 0 )
        {
            aGraphics.DrawText
                (
                    _LabelText,
                    rect._x - ( rect._width / 2F ),
                    rect._y - rect._height,
                    rect._width,
                    rect._height,
                    _FormatRect.Text.TextColor.Color,
                    _FormatRect.Text.TextFormat
                );

            //aGraphics.DrawEllipse
            //    (
            //        rect._x,
            //        rect._y,
            //        rect._width,
            //        rect._height,
            //        _FormatRect.Line.LineColor.Color,
            //        _FormatRect.Line.LineSize
            //    );

        }
        else
        {
            // 背景色
            aGraphics.FillEllipse
                (
                    rect._x,
                    rect._y,
                    rect._width,
                    rect._height,
                    _FormatRect.Background.Color
                );

            //_StopImage ??= CanvasBitmap.LoadAsync( aGraphics, new Uri( "ms-appx:///Images/test.png" ) );
            //if ( _StopImage.Status == AsyncStatus.Completed )
            //{
            //    rect.Width *= 4;
            //    rect.Height *= 4;
            //    rect._x -= ( rect._width / 2F );
            //    rect._y -= rect._height;


            //    aGraphics.DrawImage
            //        (
            //            _StopImage.GetResults(),
            //            rect
            //        );
            //}

        }
    }

    private static IAsyncOperation<CanvasBitmap>? _StopImage;


    /// <summary>
    /// ノート描画順 並替用
    /// </summary>
    /// <param name="aOther"></param>
    /// <returns></returns>
    public int CompareTo( DmsItemNote? aOther )
    {
        if ( aOther == null )
        {
            return 1;
        }
        else if ( _DrawRect.X > aOther._DrawRect.X )
        {
            return 1;
        }
        else if ( _DrawRect.X == aOther._DrawRect.X )
        {
            return 0;
        }
        return -1;
    }

    /// <summary>
    /// ノート描画順 並替用
    /// </summary>
    /// <param name="aOther"></param>
    /// <returns></returns>
    public int CompareTo( object? aOther )
    {
        return aOther == null
            ? 1
            : GetType() != aOther.GetType()
            ? throw new ArgumentException( "Invalid aOther", nameof( aOther ) )
            : CompareTo( aOther as DmsItemNote );
    }
}
