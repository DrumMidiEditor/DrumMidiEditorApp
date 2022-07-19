﻿using System;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pDMS;
using DrumMidiEditorApp.pGeneralFunction.pUtil;

namespace DrumMidiEditorApp.pView.pEditer.pEdit;

/// <summary>
/// エディター描画アイテム：BPM
/// </summary>
public class DmsItemBpm : DisposeBaseClass, IComparable, IComparable<DmsItemBpm>
{
	/// <summary>
	/// 描画範囲
	/// </summary>
	private Rect _DrawRect = new();

	/// <summary>
	/// BPM情報
	/// </summary>
	private InfoBpm? _BpmInfo = null;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="aX">描画位置＋１小節内での相対X座標</param>
	/// <param name="aY">描画位置＋１小節内での相対Y座標</param>
	/// <param name="aWidth">横幅</param>
	/// <param name="aHeight">高さ</param>
	/// <param name="aInfo">BPM情報</param>
	public DmsItemBpm( float aX, float aY, float aWidth, float aHeight, InfoBpm aInfo )
    {
		_DrawRect.X			= aX;
		_DrawRect.Y			= aY;
		_DrawRect.Width		= aWidth;
		_DrawRect.Height	= aHeight;
		_BpmInfo			= aInfo;
    }

	protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
				// Dispose managed resources.
				_BpmInfo = null;
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
        if ( _BpmInfo == null )
        {
            return;
        }

		var format = _BpmInfo.Selected ? Config.Editer.BpmSelectRect : Config.Editer.BpmNonSelectRect ;

		// 描画範囲の左上の座標基準
		var rect = _DrawRect;
		rect.X  += aDiffX;
		rect.Y  += aDiffY;

		// 背景色
	    aGraphics.FillRectangle( rect, format.BackColor );

		rect.X		+= rect.Width + 1;
		rect.Width	+= 100;

		// テキスト描画
		aGraphics.DrawText
            (
				$"{_BpmInfo.Bpm}",
				rect,
				format.TextColor,
				format.TextFormat
			);
    }

	/// <summary>
	/// BPM描画順 並替用
	/// </summary>
	/// <param name="aOther"></param>
	/// <returns></returns>
	public int CompareTo( DmsItemBpm? aOther )
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
	/// BPM描画順 並替用
	/// </summary>
	/// <param name="aOther"></param>
	/// <returns></returns>
	public int CompareTo( object? aOther )
	{
		if ( aOther == null )
		{
			return 1;
		}
		if ( GetType() != aOther.GetType() )
		{
            throw new ArgumentException( "Invalid aOther", nameof( aOther ) );
		}
		return CompareTo( aOther as DmsItemBpm );
	}
}
