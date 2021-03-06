using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation;
using Windows.UI;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pControl;
using DrumMidiEditorApp.pGeneralFunction.pLog;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;

namespace DrumMidiEditorApp.pView.pEditer.pMusic;

public sealed partial class UserControlEqualizer : UserControl
{
    /// <summary>
    /// イコライザ設定
    /// </summary>
    private ConfigEqualizer DrawSet => Config.Equalizer;

    /// <summary>
    /// イコライザ入力エリア
    /// </summary>
    private Rect _EqulizerBodyRange = new();

    /// <summary>
    /// 移動中のイコライザ入力インデックス
    /// </summary>
    private int _GainMoveIndex = -1;

    /// <summary>
    /// イコライザ入力（座標）
    /// </summary>
    private readonly List<Point> _GainCenters = new();

    /// <summary>
    /// 波形描画用のタイマー
    /// </summary>
    private PeriodicTimer? _Timer = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserControlEqualizer()
    {
        InitializeComponent();

        // イコライザ描画範囲初期化
        UpdateRange();
    }

    /// <summary>
    /// BGMへイコライザ入力内容を適用
    /// </summary>
    public void ApplyEqulizer() => UpdateEqualizer();

	/// <summary>
	/// イコライザの入力およびBGMのイコライザ設定をリセット
	/// </summary>
	public void ResetEqulizer() 
	{
		ResetEqualizer();
		Refresh();
	}

    #region CommanBar Event

    /// <summary>
    /// イコライザリセット
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EqualizerResetAppBarButton_Click( object sender, RoutedEventArgs args )
    {
		try
		{
			ResetEqulizer();
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

    /// <summary>
    /// イコライザON/OFFの切替
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EqualizerAppBarToggleButton_CheckChanged( object sender, RoutedEventArgs args )
    {
		try
		{
            ApplyEqulizer();
        }
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

    /// <summary>
    /// WaveForm ON/OFFの切替
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EqualizerWaveFormAppBarToggleButton_CheckChanged( object sender, RoutedEventArgs args )
	{
		try
		{
            // 波形描画用のタイマー設定
            if ( DrawSet.WaveFormOn )
            {
                StartWaveFormAsync();
            }
            else
            {
                StopWaveForm();
            }
        }
        catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

    /// <summary>
    /// WaveForm描画開始
    /// </summary>
    private async void StartWaveFormAsync()
    {
        if ( _Timer != null )
        {
            return;
        }

        _Timer = new( TimeSpan.FromSeconds( DrawSet.WaveFormDrawInterval ) );

        while ( await _Timer.WaitForNextTickAsync() )
        {
            Refresh();
        }
    }

    /// <summary>
    /// WaveForm描画停止
    /// </summary>
    private void StopWaveForm()
    {
        _Timer?.Dispose();
        _Timer = null;
    }

    #endregion

    #region Canvas Mouse Event

    /// <summary>
    /// マウスアクション
    /// </summary>
    private enum EActionState : int
    {
        None = 0,
        MovePoint,
        AddPoint,
        RemovePoint,
    }

    /// <summary>
    /// マウスアクション
    /// </summary>
    private EActionState _ActionState = EActionState.None;

    /// <summary>
    /// マウスダウン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EqualizerCanvas_PointerPressed( object sender, PointerRoutedEventArgs args )
	{
        if ( _ActionState != EActionState.None )
        {
            return;
        }

        try
        {
            var p = args.GetCurrentPoint( sender as FrameworkElement );

            if ( p.Properties.IsLeftButtonPressed )
            {
                if ( CheckEqulizerBodyArea( p.Position ) )
                {
                    _GainMoveIndex = GetGainCentersIndex( p.Position );

                    if ( _GainMoveIndex != -1 )
                    {
                        _ActionState = EActionState.MovePoint;
                    }
                    else
                    {
                        _ActionState = EActionState.AddPoint;
                    }

                    Refresh();
                }
            }
            else if ( p.Properties.IsRightButtonPressed )
            { 
                if ( CheckEqulizerBodyArea( p.Position ) )
                {
                    _GainMoveIndex = GetGainCentersIndex( p.Position );

                    _ActionState = EActionState.RemovePoint;

                    Refresh();
                }
            }
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );

            _ActionState = EActionState.None;
        }
    }

    /// <summary>
    /// マウス移動
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
	private void EqualizerCanvas_PointerMoved( object sender, PointerRoutedEventArgs args )
	{
        if ( _ActionState == EActionState.None )
        {
            return;
        }

        try
        {
            var p = args.GetCurrentPoint( sender as FrameworkElement );

            switch ( _ActionState )
            {
                case EActionState.MovePoint:
                    {
                        EditPointMove( _GainMoveIndex, p.Position );
                        Refresh();
                    }
                    break;
            }
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );

            _ActionState = EActionState.None;
        }
    }

    /// <summary>
    /// マウスアップ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
	private void EqualizerCanvas_PointerReleased( object sender, PointerRoutedEventArgs args )
	{
        if ( _ActionState == EActionState.None )
        {
            return;
        }

        try
        {
            var p = args.GetCurrentPoint( sender as FrameworkElement );

            switch ( _ActionState )
            {
                case EActionState.AddPoint:
                    {
                        if ( CheckEqulizerBodyArea( p.Position ) )
                        {
                            EditPoint( p.Position, true );

                            _GainMoveIndex = -1;

                            Refresh();
                        }
                    }
                    break;
                case EActionState.MovePoint:
                    {
                        EditPointMove( _GainMoveIndex, p.Position );

                        _GainMoveIndex = -1;

                        Refresh();
                    }
                    break;
                case EActionState.RemovePoint:
                    {
                        if ( CheckEqulizerBodyArea( p.Position ) )
                        {
                            EditPoint( p.Position, false );

                            _GainMoveIndex = -1;

                            Refresh();
                        }
                    }
                    break;
            }
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
        finally
        {
            _ActionState = EActionState.None;
        }
    }

    #endregion

    #region Check

    /// <summary>
    /// イコライザ入力範囲チェック
    /// </summary>
    /// <param name="aMousePos"></param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckEqulizerBodyArea( Point aMousePos )
        => XamlHelper.CheckRange( aMousePos, _EqulizerBodyRange );

    #endregion

    #region Calc

    /// <summary>
    /// マウス位置のイコライザ入力インデックスを取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>取得：0-、未取得：-1</returns>
    private int GetGainCentersIndex( Point aMousePos )
    {
        var pointRect = new Rect( 0, 0, DrawSet.PointSize, DrawSet.PointSize );

        for ( int index = _GainCenters.Count - 1; index >= 0; index-- )
        {
            var p = _GainCenters[ index ];

            pointRect.X = p.X - pointRect.Width  / 2;
            pointRect.Y = p.Y - pointRect.Height / 2;

            if ( XamlHelper.CheckRange( aMousePos, pointRect ) )
            {
                return index;
            }
        }
		return -1;
	}

    /// <summary>
    /// マウス位置の音量増減値取得
    /// </summary>
    /// <param name="aMousePosY">マウス位置</param>
    /// <returns>音量増減値</returns>
    private double CalcGain( double aMousePosY )
    {
        return DrawSet.DbGainMax -
                ( DrawSet.DbGainMax - DrawSet.DbGainMin ) *
                ( aMousePosY - _EqulizerBodyRange.Y ) / _EqulizerBodyRange.Height;
    }

    /// <summary>
    /// マウス位置のHz値取得
    /// </summary>
    /// <param name="aMousePosX">マウス位置</param>
    /// <returns>Hz値</returns>
    private double CalcHz( double aMousePosX )
    {
        double hz = 0;

        double x = aMousePosX - _EqulizerBodyRange.X;

        foreach ( var item in DrawSet.HzList )
        {
            if ( x == item.Width )
            {
                return item.Hz;
            }
            else if ( x < item.Width )
            {
                return hz + ( item.Hz - hz ) * ( x / item.Width );
            }

            hz = item.Hz;
            x -= item.Width;
        }

        return 0;
    }

    #endregion

    #region Edit

    /// <summary>
    /// イコライザ入力追加／削除
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aAddFlag">True:追加、False:削除</param>
    private void EditPoint( Point aMousePos, bool aAddFlag )
	{
        var idx = GetGainCentersIndex( aMousePos );

        if ( idx != -1 )
        {
            if ( aAddFlag )
            {
                _GainCenters[ idx ] = aMousePos;
            }
            else
            {
                _GainCenters.RemoveAt( idx );
            }
        }
        else
        {
            if ( aAddFlag )
            {
                _GainCenters.Add( aMousePos );
            }
        }

        UpdateEqualizer();
    }

    /// <summary>
    /// イコライザ入力移動
    /// </summary>
    /// <param name="aGearIndex">イコライザ入力インデックス</param>
    /// <param name="aMousePos">マウス位置</param>
    private void EditPointMove( int aGearIndex, Point aMousePos )
    {
        if ( aGearIndex == -1 )
        {
            return;
        }

        _GainCenters[ aGearIndex ] = XamlHelper.AdjustRangeIn( aMousePos, _EqulizerBodyRange );

        UpdateEqualizer();
    }

    /// <summary>
    /// BGMイコライザ設定反映
    /// </summary>
    public void UpdateEqualizer()
	{
		try
		{
            if ( DrawSet.EqualizerOn )
			{
				for ( int index = 0; index < _GainCenters.Count; index++ )
				{
                    var p = _GainCenters[ index ];

                    DmsControl.AudioData?.SetEqualizerGain( index, (float)CalcHz( p.X ), (float)CalcGain( p.Y ) );
                }
            }
            else
			{
                for ( int index = 0; index < _GainCenters.Count; index++ )
			    {
                    var p = _GainCenters[ index ];

                    DmsControl.AudioData?.SetEqualizerGain( index, (float)CalcHz( p.X ), 0 );
                }
            }

            DmsControl.AudioData?.UpdateEqualizer();
        }
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

    /// <summary>
    /// イコライザ設定リセット
    /// </summary>
    public void ResetEqualizer()
    {
        try
        {
            for ( int index = 0; index < _GainCenters.Count; index++ )
			{
                var p = _GainCenters[ index ];

                DmsControl.AudioData?.SetEqualizerGain( index, (float)CalcHz( p.X ), 0 );
            }

            DmsControl.AudioData?.UpdateEqualizer();

            _GainCenters.Clear();
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    #endregion

    #region Update

    /// <summary>
    /// フレーム処理
    /// </summary>
    private void OnMove()
    {
    }

    /// <summary>
    /// 描画範囲更新
    /// </summary>
    private void UpdateRange()
	{
        // Equlizer body
        _EqulizerBodyRange.X		= DrawSet.EqulizerBodyMarginLeftTop.X;
        _EqulizerBodyRange.Y		= DrawSet.EqulizerBodyMarginLeftTop.Y;
        _EqulizerBodyRange.Width	= DrawSet.GetHzTotalWidth();
        _EqulizerBodyRange.Height	= DrawSet.GetDbTotalHeight();

        // イコライザキャンバスのサイズ調整
        _EqualizerCanvas.Width      = DrawSet.EqulizerBodyMarginLeftTop.X + _EqulizerBodyRange.Width  + DrawSet.EqulizerBodyMarginRightBottom.X;
        _EqualizerCanvas.Height     = DrawSet.EqulizerBodyMarginLeftTop.Y + _EqulizerBodyRange.Height + DrawSet.EqulizerBodyMarginRightBottom.Y;
	}

    #endregion

    #region Draw

    /// <summary>
    /// イコライザキャンバス描画更新
    /// </summary>
    public void Refresh() => _EqualizerCanvas.Invalidate();

    /// <summary>
    /// イコライザキャンバス描画
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EqualizerCanvas_Draw( CanvasControl sender, CanvasDrawEventArgs args )
    {
        try
        {
            OnMove();

            var body = _EqulizerBodyRange;

            #region x-line (Hz)
            {
                var pos_x = body.X;

                foreach ( var item in DrawSet.HzList )
                {
                    pos_x += item.Width;

                    args.DrawingSession.DrawText
                        (
                            item.LabelName,
                            (float)( pos_x ),
                            (float)( body.Bottom + DrawSet.XLineTitlePaddingBottom ),
                            DrawSet.TextColor,
                            DrawSet.TextFormatCenter
                        );

                    args.DrawingSession.DrawLine
                        (
                            (float)( pos_x ),
                            (float)( body.Top ),
                            (float)( pos_x ),
                            (float)( body.Bottom + DrawSet.XLineTitlePaddingBottom ),
                            DrawSet.LineColor
                        );
                }
            }
            #endregion

            #region y-line (db)
            {
                var pos_y = body.Y;

                for ( int y = 0; y <= DrawSet.DbGainSeparateHeightCount; y++ )
                {
                    var db = DrawSet.DbGainMax - DrawSet.DbGainSeparate * y;

                    args.DrawingSession.DrawText
                        (
                            $"{db}db",
                            (float)( body.Left - DrawSet.YLineTitlePaddingRight ),
                            (float)( pos_y ),
                            DrawSet.TextColor,
                            DrawSet.TextFormatRight
                        );

                    args.DrawingSession.DrawLine
                        (
                            (float)( body.Left - DrawSet.YLineTitlePaddingRight ),
                            (float)( pos_y ),
                            (float)( body.Right ),
                            (float)( pos_y ),
                            DrawSet.LineColor
                        );

                    pos_y += DrawSet.DbGainSeparateHeightTermSize;
                }
            }
            #endregion

            #region 周波数解析

            // TODO: 仮作成。BGMの再読み込み時にエラーになるので使う場合は改良が必要

            if ( DrawSet.WaveFormOn )
            {
                var bgm = DmsControl.AudioData;

                if ( bgm?.IsEnableFFT() ?? false )
                {
                    var fft = bgm.GetFFTPlaying();

                    var hzPerOne = (float)bgm.GetSampleRate() / ( fft.Count * bgm.Channels );

                    var pen = new Color[ 2 ]
                        {
                            DrawSet.WaveLeftColor,
                            DrawSet.WaveRightColor,
                        };

                    var r = new Rect( 0, 0, 2, 1 );

                    for ( int k = 0; k < fft.Count; k++ )
                    {
                        r.X         = body.X - k % bgm.Channels;
                        r.Height    = fft[k] * body.Height;
                        r.Y         = body.Y + body.Height - ( r.Height > 0 ? r.Height : 0 );

                        var hz_b    = 0d;
                        var hz      = hzPerOne * ( k + 1 - k % bgm.Channels );

                        foreach ( var item in DrawSet.HzList )
                        {
                            if ( hz > item.Hz )
                            {
                                r.X += item.Width;
                                hz_b = item.Hz;
                            }
                            else
                            {
                                r.X += item.Width * ( hz - hz_b ) / ( item.Hz - hz_b );
                                break;
                            }
                        }

                        args.DrawingSession.DrawLine
                            (
                                r._x,
                                r._y,
                                r._x,
                                (float)r.Bottom,
                                pen[ k % bgm.Channels ]
                            );
                    }
                }
            }
            #endregion

            #region point
            {
                for ( int index = 0; index < _GainCenters.Count; index++ )
                {
                    var point = _GainCenters[ index ];

                    args.DrawingSession.DrawCircle
                        (
                            point._x,
                            point._y,
                            (float)( DrawSet.PointSize / 2d ),
                            DrawSet.PointColor
                        );

                    args.DrawingSession.FillCircle
                        (
                            point._x,
                            point._y,
                            (float)( DrawSet.PointSize / 2d ),
                            index == _GainMoveIndex ? DrawSet.PointSelectColor : DrawSet.PointNonSelectColor
                        );
                }
            }
            #endregion
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    /// <summary>
    /// Win2D アンロード処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Page_Unloaded( object sender, RoutedEventArgs args )
    {
        try
        {
            // Win2D アンロード
            //_EqualizerCanvas.RemoveFromVisualTree();
            //_EqualizerCanvas = null;
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    #endregion
}
