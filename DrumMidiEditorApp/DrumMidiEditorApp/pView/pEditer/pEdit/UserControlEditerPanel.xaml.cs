﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

using DrumMidiEditorApp.pAudio;
using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pControl;
using DrumMidiEditorApp.pDMS;
using DrumMidiEditorApp.pGeneralFunction.pAudio;
using DrumMidiEditorApp.pGeneralFunction.pLog;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;
using DrumMidiEditorApp.pResume;
using Microsoft.UI.Windowing;

namespace DrumMidiEditorApp.pView.pEditer.pEdit;

public sealed partial class UserControlEditerPanel : UserControl
{
    #region Member

    /// <summary>
    /// リソース
    /// </summary>
    private readonly ResourceLoader _Resource = new();

    /// <summary>
    /// Editerタブ設定
    /// </summary>
    private ConfigEditer DrawSet => Config.Editer;

    /// <summary>
    /// System設定
    /// </summary>
    private ConfigSystem ConfigSystem => Config.System;

    /// <summary>
    /// Scale設定
    /// </summary>
    private ConfigScale ConfigScale => Config.Scale;

    /// <summary>
    /// Media設定
    /// </summary>
    private ConfigMedia ConfigMedia => Config.Media;

    /// <summary>
    /// Score情報
    /// </summary>
    private Score Score => DMS.SCORE;

    /// <summary>
    /// Score予測情報
    /// </summary>
    private Score ScorePredict => DMS.SCORE_PREDICT;

    /// <summary>
    /// 編集履歴管理
    /// </summary>
    private readonly ResumeManage _EditResumeMng = new();

    /// <summary>
    /// パネルスクリーンサイズ
    /// </summary>
    private Size _ScreenSize = new();

    /// <summary>
    /// 情報テキスト表示範囲
    /// </summary>
	private Rect _InfoRange = new();

    /// <summary>
    /// BPM行ヘッダ範囲
    /// </summary>
	private Rect _BpmHeadRange = new();

    /// <summary>
    /// BPM行ボディ範囲
    /// </summary>
	private Rect _BpmBodyRange = new();

    /// <summary>
    /// 小節番号行ヘッダ範囲
    /// </summary>
	private Rect _MeasureNoHeadRange = new();

    /// <summary>
    /// 小節番号行ボディ範囲
    /// </summary>
	private Rect _MeasureNoBodyRange = new();

    /// <summary>
    /// DrumGroup/Drum行ヘッダ範囲
    /// </summary>
    private Rect _ScoreHeadRange = new();

    /// <summary>
    /// DrumGroup/Drum行ボディ範囲
    /// </summary>
    private Rect _ScoreBodyRange = new();

    /// <summary>
    /// 音量行ヘッダ範囲
    /// </summary>
    private Rect _VolumeHeadRange = new();

    /// <summary>
    /// 音量行ボディ範囲
    /// </summary>
    private Rect _VolumeBodyRange = new();

    /// <summary>
    /// MidiMapGroup/MidiMap行の表示高さ。
    /// （Volume行が被さっている場合、その高さが差し引かれる）
    /// </summary>
    private float _ScoreViewHeight = 0;

    /// <summary>
    /// マウスダウン押下時のマウス位置
    /// </summary>
    private Point _MouseDownPosition = new();

    /// <summary>
    /// マウスダウン押下時のマウス位置にあるノート位置（絶対値）
    /// </summary>
    private PointInt _NoteDownPosition = new();

    /// <summary>
    /// マウス位置にあるノート位置（絶対値）
    /// </summary>
    private PointInt _NoteCursorPosition = new( -1, -1 );

    /// <summary>
    /// ノートコピー範囲内で左端ノートのノート位置（絶対値）
    /// </summary>
	private int	_CopyNotePositionX = 0;

    /// <summary>
    /// コピーしたNOTE情報リスト
    /// </summary>
	private readonly List<InfoNote> _CopyNoteList = new();

    /// <summary>
    /// コピーしたBPM情報リスト
    /// </summary>
	private readonly List<InfoBpm> _CopyBpmList = new();

    /// <summary>
    /// 五線リスト
    /// </summary>
	private readonly List<DmsItemLine> _StaffLineList = new();

    /// <summary>
    /// 小節線リスト
    /// </summary>
    private readonly List<DmsItemLine> _MeasureLineList	= new();

    /// <summary>
    /// MidiMapGroup/MidiMapヘッダリスト
    /// </summary>
    private readonly List<DmsItemMidiMap> _HeaderList = new();

    /// <summary>
    /// BPMリスト＜小節番号、アイテム＞
    /// </summary>
    private readonly Dictionary<int,List<DmsItemBpm>> _BpmList = new();

    /// <summary>
    /// ノートリスト＜小節番号、アイテム＞
    /// </summary>
    private readonly Dictionary<int,List<DmsItemNote>> _NoteList = new();

    /// <summary>
    /// ノート予測リスト＜小節番号、アイテム＞
    /// </summary>
    private readonly Dictionary<int,List<DmsItemNote>> _NotePredictList = new();

    /// <summary>
    /// ノート音量リスト＜小節番号、アイテム＞
    /// </summary>
    private readonly Dictionary<int,List<DmsItemNoteVolume>> _VolumeList = new();

    /// <summary>
    /// ノート背景色リスト＜MidiMapKey、背景色＞
    /// </summary>
    private readonly Dictionary<int, FormatRect> _MidiMapNoteFormatList = new();

    /// <summary>
    /// サポート線
    /// </summary>
    private readonly DmsItemSupportLine _SupportLine = new();

    /// <summary>
    /// ノート範囲選択
    /// </summary>
    private DmsItemNoteRange _NoteRange = new();

    /// <summary>
    /// ノート範囲選択（前回値）
    /// </summary>
    private readonly DmsItemNoteRange _NoteRangeBef = new();

    /// <summary>
    /// 音量入力
    /// </summary>
    private readonly DmsItemVolumeRange _VolumeRange = new();

    /// <summary>
    /// 音階別周波数表示用BGM
    /// </summary>
    private NAudioData? _ScaleBgm = null;

    /// <summary>
    /// 音階別周波数表示用Bitmap
    /// </summary>
    private CanvasBitmap? _ScaleBitmap = null;

    /// <summary>
    /// 等間隔処理実行用タイマー
    /// </summary>
    private PeriodicTimer? _Timer = null;

    /// <summary>
    /// マウス位置：ノート範囲移動時のタイマー処理用
    /// </summary>
    private Point _MoveMousePoint = new();

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserControlEditerPanel()
    {
        InitializeComponent();
    }

    #region Mouse Event

    /// <summary>
    /// アクション状態一覧
    /// </summary>
    private enum EActionState
    {
        None = 0,
        EditVolume,
        AddBpm,
        RemoveBpm,
        AddNote,
        RemoveNote,
        EditSupportLine,
        EditSelectDrum,
        SelectNoteRange,
        MoveNoteRange,
        RemoveNoteRange,
    }

    /// <summary>
    /// アクション状態
    /// </summary>
    private EActionState _ActionState = EActionState.None;

    /// <summary>
    /// マウスダウン処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EditerCanvas_PointerPressed( object sender, PointerRoutedEventArgs args )
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
                if ( CheckVolumeBodyArea( p.Position ) )
                {
                    EditVolumeRange( p.Position, true );

    			    _ActionState = EActionState.EditVolume;
                }
                else if ( CheckMeasureNoBodyArea( p.Position ) )
                {
                    EditSupportRange( p.Position, true );

                    _ActionState = EActionState.EditSupportLine;
                }
                else if ( CheckScoreHeadArea( p.Position ) )
                {
    			    _ActionState = EActionState.EditSelectDrum;
                }
                else if ( CheckBpmBodyArea( p.Position ) )
                {
					_NoteRangeBef.Set( _NoteRange );

					if ( _NoteRange.Selected )
					{
                        ClearSelectNoteRange();

						_ActionState = EActionState.None;
					}
					else
					{
                        _ActionState = EActionState.AddBpm;
                    }
                }
                else if ( CheckNoteRangeArea( p.Position ) )
                {
    				_NoteRangeBef.Set( _NoteRange );

                    if ( ( args.KeyModifiers & VirtualKeyModifiers.Control ) == VirtualKeyModifiers.Control )
					{
                        CopyNoteRange();
					}
					else
					{
                        EditMoveNoteRange( p.Position, true );

						_ActionState = EActionState.MoveNoteRange;
					}
				}
                else if ( CheckScoreBodyArea( p.Position ) )
                {
					_NoteRangeBef.Set( _NoteRange );

                    if ( ( args.KeyModifiers & VirtualKeyModifiers.Control ) == VirtualKeyModifiers.Control )
					{
                        PasteNoteRange( p.Position );
                    }
                    else
					{
						if ( _NoteRange.Selected )
						{
                            ClearSelectNoteRange();

							_ActionState = EActionState.None;
						}
						else
						{
							_MouseDownPosition = p.Position;
							_NoteDownPosition  = CalcNotePosition( p.Position );

							_ActionState = EActionState.AddNote;
						}
					}
                }
			}
            else if ( p.Properties.IsRightButtonPressed )					
			{
                if (CheckVolumeBodyArea( p.Position ) )
                {
                    ClearVolumeRange();
                }
                else if ( CheckMeasureNoBodyArea( p.Position ) )
                {
                    ClearSupportRange();
                }
                else if ( CheckScoreHeadArea( p.Position ) )
                {
                }
                else if ( CheckBpmBodyArea( p.Position ) )
                {
					_NoteRangeBef.Set( _NoteRange );

                    if ( _NoteRange.Selected )
                    {
                        ClearSelectNoteRange();

                        _ActionState = EActionState.None;
                    }
                    else
                    {
                        _ActionState = EActionState.RemoveBpm;
                    }
                }
                else if ( CheckNoteRangeArea( p.Position ) )
                {
    				_NoteRangeBef.Set( _NoteRange );

                    _ActionState = EActionState.RemoveNoteRange;
                }
                else if ( CheckScoreBodyArea( p.Position ) )
                {
					_NoteRangeBef.Set( _NoteRange );

                    if ( _NoteRange.Selected )
                    {
                        ClearSelectNoteRange();

                        _ActionState = EActionState.None;
                    }
                    else
                    {
                        _MouseDownPosition = p.Position;
                        _NoteDownPosition  = CalcNotePosition( p.Position );

                        _ActionState = EActionState.RemoveNote;
                    }
                }
			}

            Refresh();
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );

            _ActionState = EActionState.None;
        }
    }

    /// <summary>
    /// マウス移動処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EditerCanvas_PointerMoved( object sender, PointerRoutedEventArgs args )
    {
        try
        {
            var p = args.GetCurrentPoint( sender as FrameworkElement );

			switch ( _ActionState )
			{
                case EActionState.None:
                    {
						if ( CheckNoteRangeArea( p.Position ) )
						{
							if ( _NoteCursorPosition.X != -1 || _NoteCursorPosition.Y != -1 )
							{
								_NoteCursorPosition.X = -1;
								_NoteCursorPosition.Y = -1;
							}
						}
						else if ( CheckBpmBodyArea( p.Position ) ||
                            CheckMeasureNoBodyArea( p.Position ) ||
                            CheckScoreBodyArea( p.Position ) ||
                            CheckVolumeBodyArea( p.Position ) )
						{
							var note_pos = CalcNotePosition( p.Position );

							if ( _NoteCursorPosition != note_pos )
							{
								_NoteCursorPosition = note_pos;
							}
						}
                    }
                    break;
                case EActionState.EditVolume:
                    {
                        EditVolumeRange( p.Position, false );
                    }
                    break;
                case EActionState.EditSupportLine:
                    {
                        EditSupportRange( p.Position, false );
                    }
                    break;
                case EActionState.AddBpm:
                    return;
                case EActionState.AddNote:
                    {
                        var note_pos = CalcNotePosition( p.Position );

                        if ( note_pos == _NoteDownPosition )
                        {
                            _ActionState = EActionState.AddNote;
                        }
                        else
                        {
                            EditNoteRange( _MouseDownPosition, true );
                            EditNoteRange( p.Position, false );

                            _ActionState = EActionState.SelectNoteRange;
                        }
                    }
                    break;
                case EActionState.SelectNoteRange:
                    {
                        EditNoteRange( p.Position, false );

                        MoveNoteRangeAsync( p.Position );
                    }
                    break;
                case EActionState.MoveNoteRange:
                    {
                        EditMoveNoteRange( p.Position, false );

                        MoveNoteRangeAsync( p.Position );
                    }
                    break;
    		}

            Refresh();
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );

            _ActionState = EActionState.None;
        }
    }

    /// <summary>
    /// マウスアップ処理
    /// 
    /// https://docs.microsoft.com/ja-jp/windows/apps/design/input/handle-pointer-input
    /// PointerPressed イベントと PointerReleased イベントは、常にペアで発生するわけではありません。
    /// アプリでは、ポインターの押下を終了させる可能性のあるすべてのイベント
    /// (PointerExited、PointerCanceled、PointerCaptureLost など) をリッスンして処理する必要があります。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EditerCanvas_PointerReleased( object sender, PointerRoutedEventArgs args )
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
                case EActionState.EditVolume:
                    {
                        EditNoteVolume( p.Position );
                    }
                    break;
                case EActionState.EditSupportLine:
                    {
                        EditSupportRange( p.Position, false );
                    }
                    break;
                case EActionState.AddBpm:
                    {
                        EditBpm( p.Position, true );
                    }
                    break;
                case EActionState.AddNote:
                    {
                        EditNote( p.Position, true );
                    }
                    break;
                case EActionState.EditSelectDrum:
                    {
                        EditSelectMidiMap( p.Position );
                    }
                    break;
                case EActionState.SelectNoteRange:
                    {
                        StopTimer();

                        EditSelectNoteRange( p.Position );
                    }
                    break;
                case EActionState.MoveNoteRange:
                    {
                        StopTimer();

                        EditMoveNoteRange( p.Position );
                    }
                    break;
                case EActionState.RemoveBpm:
                    {
                        EditBpm( p.Position, false );
                    }
                    break;
                case EActionState.RemoveNote:
                    {
                        EditNote( p.Position, false );
                    }
                    break;
                case EActionState.RemoveNoteRange:
                    {
                        RemoveNoteRange();
                    }
                    break;
			}

            Refresh();
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

    /// <summary>
    /// ノート範囲移動処理
    /// </summary>
    /// <param name="aMousePoint"></param>
    private async void MoveNoteRangeAsync( Point aMousePoint )
    {
        if ( _Timer != null )
        {
            _MoveMousePoint= aMousePoint;
            return;
        }

        _Timer = new( TimeSpan.FromSeconds( DrawSet.SheetTimerSecond ) );

        while ( await _Timer.WaitForNextTickAsync() )
        {
            var mousePoint  = _MoveMousePoint;
            var note_pos    = DrawSet.NotePosition;
            var paddingSize = DrawSet.SheetMovePaddingSize;

            if ( mousePoint.Y < _ScoreBodyRange.Y + paddingSize.Height )
	        {
                note_pos.Y -= (int)( ( _ScoreBodyRange.Y + paddingSize.Height - mousePoint.Y ) / DrawSet.NoteHeightSize );

		        if ( note_pos.Y < 0 )
		        {
                    note_pos.Y = 0;
		        }
	        }
	        if ( mousePoint.Y > _ScreenSize.Height - paddingSize.Height )
	        {
                note_pos.Y += (int)( ( mousePoint.Y - _ScreenSize.Height + paddingSize.Height ) / DrawSet.NoteHeightSize );

		        if ( note_pos.Y >= Score.EditMidiMapSet.DisplayMidiMapAllCount  )
		        {
                    note_pos.Y = Score.EditMidiMapSet.DisplayMidiMapAllCount - 1;
		        }
	        }
	        if ( mousePoint.X < _ScoreBodyRange.X + paddingSize.Width )
	        {
                note_pos.X -= (int)( ( _ScoreBodyRange.X + paddingSize.Width - mousePoint.X ) / DrawSet.NoteWidthSize );

		        if ( note_pos.X < 0 )
		        {
                    note_pos.X = 0;
		        }
	        }
	        if ( mousePoint.X > _ScreenSize.Width - paddingSize.Width )
	        {
                note_pos.X += (int)( ( mousePoint.X - _ScreenSize.Width + paddingSize.Width ) / DrawSet.NoteWidthSize );

		        if ( note_pos.X >= ConfigSystem.NoteCount )
		        {
                    note_pos.X = ConfigSystem.NoteCount - 1;
		        }
	        }

            DrawSet.NotePosition = note_pos;

            Refresh();
        }
    }

    /// <summary>
    /// タイマー停止
    /// </summary>
    private void StopTimer()
    {
        _Timer?.Dispose();
        _Timer = null;
    }

    #endregion

    #region Check

    /// <summary>
    /// 小節番号行ボディ範囲チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
	private bool CheckMeasureNoBodyArea( Point aMousePos )
        => !CheckVolumeBodyArea( aMousePos ) 
            && XamlHelper.CheckRange( aMousePos, _MeasureNoBodyRange );

    /// <summary>
    /// MidiMapGroup/MidiMapヘッダ範囲チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckScoreHeadArea( Point aMousePos )
        => !CheckVolumeBodyArea( aMousePos ) 
            && XamlHelper.CheckRange( aMousePos, _ScoreHeadRange );

    /// <summary>
    /// MidiMapGroup/MidiMapボディ範囲チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckScoreBodyArea( Point aMousePos )
        => !CheckVolumeBodyArea( aMousePos ) 
            && XamlHelper.CheckRange( aMousePos, _ScoreBodyRange );

    /// <summary>
    /// BPM行ボディ範囲チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckBpmBodyArea( Point aMousePos )
        => !CheckVolumeBodyArea( aMousePos ) 
        && XamlHelper.CheckRange( aMousePos, _BpmBodyRange );

    /// <summary>
    /// 音量行ボディ範囲チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckVolumeBodyArea( Point aMousePos )
        => DrawSet.VolumeDisplay 
            && XamlHelper.CheckRange( aMousePos, _VolumeBodyRange );

    /// <summary>
    /// 範囲選択内チェック
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>True:範囲内、False:範囲外</returns>
    private bool CheckNoteRangeArea( Point aMousePos )
        => !CheckVolumeBodyArea( aMousePos )
            && XamlHelper.CheckRange( aMousePos, _NoteRange.GetSelectRange( DrawSet.NotePosition ) );

    #endregion

    #region Calc

    /// <summary>
    /// MidiMap位置取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>X=-1:未取得、0:MidiMapGroup、1:MidiMap、Y=-1:未取得、0-n=MidiMapGroup/MidiMap表示連番</returns>
    private PointInt CalcMidiMapPosition( Point aMousePos )
    {
		var	head		= _ScoreHeadRange;
		var	note_pos    = DrawSet.NotePosition;

        var pos = new PointInt()
        {
            X = (int)( ( aMousePos.X - head.X ) / ( DrawSet.HeaderWidthSize / 2 ) ),
            Y = (int)( note_pos.Y + ( aMousePos.Y - head.Y ) / DrawSet.NoteHeightSize )
        };

        if ( pos.X < 0 || 2 <= pos.X )
        {
            pos.X = -1;
        }

        if ( pos.Y < 0 || Score.EditMidiMapSet.DisplayMidiMapAllCount <= pos.Y )
		{
			pos.Y = -1;
		}

        return pos;
	}

    /// <summary>
    /// １小節分割数を考慮したノート位置（絶対値）を取得
    /// </summary>
    /// <param name="aAbsolutePosX">ノート位置（絶対値）</param>
    /// <returns>計算後ノート位置（絶対値）</returns>
    private int CalcNotePosition( int aAbsolutePosX )
    {
        var sa = aAbsolutePosX % DrawSet.DivisionLineCount;

        return aAbsolutePosX - sa + ( ( sa <= DrawSet.DivisionLineCount / 2 ) ? 0 : DrawSet.DivisionLineCount );
    }

    /// <summary>
    /// ノート位置（絶対値）取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>X=-1:未取得、0-n:ノート位置（絶対値）、Y=-1:未取得、0-n=MidiMapGroup/MidiMap表示連番</returns>
    private PointInt CalcNotePosition( Point aMousePos )
    {
		var	body	    = _ScoreBodyRange;
		var note_pos    = DrawSet.NotePosition;

        var pos = new PointInt()
        { 
			X = (int)( note_pos.X + ( aMousePos.X - body.X + DrawSet.NoteWidthSize / 2 ) / DrawSet.NoteWidthSize ),
			Y = (int)( note_pos.Y + ( aMousePos.Y - body.Y ) / DrawSet.NoteHeightSize ),
        };

        pos.X = CalcNotePosition( pos.X );

        if ( pos.X < 0 || ConfigSystem.NoteCount <= pos.X )
        {
            pos.X = -1;
        }

        if ( pos.Y < 0 || Score.EditMidiMapSet.DisplayMidiMapAllCount <= pos.Y )
		{
			pos.Y = -1;
		}

		return pos;
	}

    /// <summary>
    /// BPM位置（絶対値）取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>X=-1:未取得、0-n:NOTE位置（絶対値）、Y=0</returns>
    private PointInt CalcBpmPosition( Point aMousePos )
    {
		var	note_pos    = DrawSet.NotePosition;
		var	body		= _BpmBodyRange;

        var pos = new PointInt()
        {
            X = (int)( note_pos.X + ( aMousePos.X - body.X + DrawSet.NoteWidthSize / 2 ) / DrawSet.NoteWidthSize ),
            Y = 0
        };

        pos.X = CalcNotePosition( pos.X );

        if ( pos.X < 0 || ConfigSystem.NoteCount <= pos.X )
        {
            pos.X = -1;
        }

		return pos;
	}

    /// <summary>
    /// 小節番号位置（絶対値）取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>X=-1:未取得、0-n:NOTE位置（絶対値）、Y=0</returns>
    private PointInt CalcMeasureNoPosition( Point aMousePos )
    {
        var note_pos    = DrawSet.NotePosition;
        var body        = _MeasureNoBodyRange;

        var pos = new PointInt()
        {
            X = (int)( note_pos.X + ( aMousePos.X - body.X + DrawSet.NoteWidthSize / 2 ) / DrawSet.NoteWidthSize ),
            Y = 0
        };

        pos.X = CalcNotePosition( pos.X );

        if ( pos.X < 0 || ConfigSystem.NoteCount <= pos.X )
        {
            pos.X = -1;
        }

        return pos;
    }

    /// <summary>
    /// ノート音量位置（絶対値）取得
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <returns>X=-1:未取得、0-n:ノート位置（絶対値）、Y=音量(127基準)</returns>
    private PointInt CalcNoteVolumePosition( Point aMousePos )
    {
		var	note_pos    = DrawSet.NotePosition;
		var	body		= _VolumeBodyRange;

        var pos = new PointInt()
        {
            X = (int)( note_pos.X + ( aMousePos.X - body.X + DrawSet.NoteWidthSize / 2 ) / DrawSet.NoteWidthSize ),
            Y = (int)( body.Bottom - aMousePos.Y ),
        };

        if ( pos.X < 0 || ConfigSystem.NoteCount <= pos.X )
        {
            pos.X = -1;
        }

        pos.Y = ConfigMedia.CheckMidiVolume( pos.Y );

		return pos;
	}

    #endregion

    #region Edit

    /// <summary>
    /// BPM編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aAddFlag">True:追加/変更、False:削除</param>
    private void EditBpm( Point aMousePos, bool aAddFlag )
    {
        var pos = CalcBpmPosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        int measure_no  = pos.X / ConfigSystem.MeasureNoteNumber;
        int note        = pos.X % ConfigSystem.MeasureNoteNumber;

        var info_old = Score.SysChannel.GetBpm( measure_no, note );

        var rs = new ResumeMultiple();

        #region Input bpm
        if ( aAddFlag )
        {
            InfoBpm info_new;

            if ( info_old == null )
            {
                info_new = new( measure_no, note )
                {
                    Bpm = Score.Bpm
                };
            }
            else
            {
                info_new = info_old.Clone();
            }

            var page = new PageInputBpm
            {
                Bpm = info_new.Bpm
            };

            XamlHelper.InputDialogOkCancelAsync
                (
                    Content.XamlRoot,
                    ResourcesHelper.GetString( "LabelBpm" ),
                    page,
                    new(() =>
                    {
                        info_new.Bpm = page.Bpm;

                        rs.AddBpm( info_old, info_new );

                        _EditResumeMng.ExcuteAndResume( rs );

                        UpdateBpmMeasure( measure_no );

                        Refresh();
                    })
                );

            return;
        }
        #endregion
        #region Remove bpm
        else
        {
            if ( info_old == null )
            {
                return;
            }

            rs.RemoveBpm( info_old );
        }
        #endregion

        _EditResumeMng.ExcuteAndResume( rs );

        UpdateBpmMeasure( measure_no );
    }

    /// <summary>
    /// サポート線クリア
    /// </summary>
    private void ClearSupportRange() => _SupportLine.ClearPos();

    /// <summary>
    /// サポート線の編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aStart">True:入力開始、False:入力途中/終了</param>
    private void EditSupportRange( Point aMousePos, bool aStart )
    {
        var pos = CalcMeasureNoPosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        if ( aStart )
        {
            _SupportLine.SetStartPos( pos.X );
        }
        else
        {
            _SupportLine.SetEndPos( pos.X );
        }
    }

    /// <summary>
    /// MidiMapGroup/MidiMapヘッダの選択状態編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    private void EditSelectMidiMap( Point aMousePos )
    {
        if ( !CheckScoreHeadArea( aMousePos ) )
        {
            return;
        }

        var pos = CalcMidiMapPosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        var midiMap = Score.EditMidiMapSet.DisplayMidiMaps[ pos.Y ];

        if ( midiMap == null || midiMap.Group == null )
        {
            return;
        }

        var rs = new ResumeMultiple();

        switch ( pos.X )
        {
            case 0: rs.SelectMidiMapGroup( midiMap.Group );   break;
            case 1: rs.SelectMidiMap( midiMap );              break;
            default: return;
        }

        _EditResumeMng.ExcuteAndResume( rs );

        UpdateNoteVolumeMeasure();
    }

    /// <summary>
    /// NOTE編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aAddFlag">True:追加/変更、False:削除</param>
	private void EditNote( Point aMousePos, bool aAddFlag )
	{
        if ( !CheckScoreBodyArea( aMousePos ) )
        {
            return;
        }

		var pos = CalcNotePosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        var midiMap = Score.EditMidiMapSet.DisplayMidiMaps[ pos.Y ];

        if ( midiMap == null )
        {
            return;
        }

        var rs = new ResumeMultiple();

		int measure_no = pos.X / ConfigSystem.MeasureNoteNumber;
		int note       = pos.X % ConfigSystem.MeasureNoteNumber;

        var info_old = Score.EditChannel.GetNote( midiMap.MidiMapKey, measure_no, note );

        #region Add note
        if ( aAddFlag )
        {
            InfoNote info_new;

            if ( info_old == null )
            {
                info_new = new( Score.EditChannelNo, midiMap.MidiMapKey, measure_no, note, DrawSet.NoteOn, !DrawSet.NoteOn );
            }
            else
            {
                if (    ( DrawSet.NoteOn && info_old.NoteOn == true && info_old.Volume == DrawSet.NoteSelectVolume )
                    ||  ( !DrawSet.NoteOn && info_old.NoteOff == true ) )
                {
                    return;
                }

                info_new = info_old.Clone();

                if ( DrawSet.NoteOn )
                {
                    info_new.NoteOn = true;
                }
                else
                {
                    info_new.NoteOff = true;
                }
            }

            if ( DrawSet.NoteOn )
            {
                info_new.Volume = DrawSet.NoteSelectVolume;
            }

            rs.AddNote( info_old, info_new );

            AudioFactory.SinglePlay( Score.EditChannelNo, midiMap.Midi, (byte)( midiMap.VolumeAddIncludeGroup + info_new.Volume ) );
        }
        #endregion

        #region Remove note
        else if ( info_old == null )
        {
            return;
        }
        else if ( DrawSet.NoteOn && info_old.NoteOff )
        {
            var info_new = info_old.Clone();
            info_new.NoteOn = false;

            rs.AddNote( info_old, info_new );
        }
        else if ( !DrawSet.NoteOn && info_old.NoteOn )
        {
            var info_new = info_old.Clone();
            info_new.NoteOff = false;

            rs.AddNote( info_old, info_new );
        }
        else
        { 
            rs.RemoveNote( info_old );
        }
        #endregion

        _EditResumeMng.ExcuteAndResume( rs );
    }

    /// <summary>
    /// 範囲選択解除
    /// </summary>
    private void ClearSelectNoteRange()
    {
		var rs = new ResumeMultiple();

        ClearSelectNoteRange( ref rs );

		if ( rs.Count > 0 )
        {
            _EditResumeMng.ExcuteAndResume( rs );
        }
	}

    /// <summary>
    /// 範囲選択解除。処理の実行は呼び出し元で実施が必要です。
    /// </summary>
    /// <param name="aResume">履歴</param>
	private void ClearSelectNoteRange( ref ResumeMultiple aResume )
    {
        // TODO: Editer
        //DMS.EditerForm?.EditerCtl.DoClearRangeCheckBox( false );

        if ( !_NoteRange.Selected )
        {
            return;
        }

        #region NoteRange
        {
            _NoteRange.ClearPos();

			aResume.SelectNoteRange( ref _NoteRange, _NoteRangeBef );
        }
        #endregion

        #region Cancel current selected note
        {
            foreach ( var info in Score.EditChannel.NoteInfoList.Values )
            {
                if ( info.Selected )
                {
					aResume.SelectNote( info, false );
                }
            }
        }
        #endregion

        #region Cancel current selected bpm
        {
            foreach ( var info in Score.SysChannel.BpmInfoList.Values )
            {
                if ( info.Selected )
                {
					aResume.SelectBpm( info, false );
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 範囲選択の選択
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aStart">True:入力開始、False:入力途中/終了</param>
    private void EditNoteRange( Point aMousePos, bool aStart )
    {
        var pos = CalcNotePosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        if ( aStart )
        {
            _NoteRange.SetStartPos( pos.X, pos.Y, DrawSet.RangeSelect );
        }
        else
        {
            _NoteRange.SetEndPos( pos.X, pos.Y );
        }
    }

    /// <summary>
    /// 範囲選択内のNOTE/BPM選択処理
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    private void EditSelectNoteRange( Point aMousePos )
    {
        var rs = new ResumeMultiple();

        #region NoteRange
        {
            EditNoteRange( aMousePos, false );

            rs.SelectNoteRange( ref _NoteRange, _NoteRangeBef );
        }
        #endregion

        #region Cancel current selected note
        {
            foreach ( var info in Score.EditChannel.NoteInfoList.Values )
            {
                if ( info.Selected )
                {
                    rs.SelectNote( info, false );
                }
            }
        }
        #endregion

        #region Select notes within range
        {
            int sx = _NoteRange.StartNotePosX;
            int sy = _NoteRange.StartNotePosY;
            int ex = _NoteRange.EndNotePosX;
            int ey = _NoteRange.EndNotePosY;

            int measureNoStart  = sx / ConfigSystem.MeasureNoteNumber;
            int measureNoEnd    = ex / ConfigSystem.MeasureNoteNumber; // + ( ex % Config.System.MeasureNoteNumber == 0 ? 1 : 0 );

            int notePosStart;
            int notePosEnd;

            for ( int measure_no = measureNoStart; measure_no <= measureNoEnd; measure_no++ )
            {
                var measure = Score.EditChannel.GetMeasure( measure_no );

                if ( measure == null )
                {
                    continue;
                }

                notePosStart    = ( measure_no == measureNoStart ? sx % ConfigSystem.MeasureNoteNumber : 0 );
                notePosEnd      = ( measure_no == measureNoEnd   ? ex % ConfigSystem.MeasureNoteNumber : ConfigSystem.MeasureNoteNumber - 1 );

                for ( int y = sy; y <= ey; y++ )
                {
				    var midiMap = Score.EditMidiMapSet.DisplayMidiMaps[ y ];
                        
                    if ( midiMap == null )
                    {
                        continue;
                    }

					if ( !measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var note_line ) )
					{
                        continue;
                    }

                    for ( int note_pos = notePosStart; note_pos <= notePosEnd; note_pos++ )
                    {
                        if ( !note_line.InfoStates.TryGetValue( note_pos, out var info ) )
                        {
                            continue;
                        }

                        rs.SelectNote( info, true );
                    }
                }
            }
        }
        #endregion

        #region Cancel current selected bpm
        {
            foreach ( var info in Score.SysChannel.BpmInfoList.Values )
            {
                if ( info.Selected )
                {
                    rs.SelectBpm( info, false );
                }
            }
        }
        #endregion

        if ( DrawSet.IncludeBpm )
        {
            #region Select bpms within range
            {
                int sx = _NoteRange.StartNotePosX;
                int ex = _NoteRange.EndNotePosX;

                int measureNoStart  = sx / ConfigSystem.MeasureNoteNumber;
                int measureNoEnd    = ex / ConfigSystem.MeasureNoteNumber; // + ( ex % Config.System.MeasureNoteNumber == 0 ? 1 : 0 );

                int notePosStart;
                int notePosEnd;

                for ( int measure_no = measureNoStart; measure_no <= measureNoEnd; measure_no++ )
                {
                    var measure = Score.SysChannel.GetMeasure( measure_no );

                    if ( measure == null )
                    {
                        continue;
                    }

                    notePosStart    = ( measure_no == measureNoStart ? sx % ConfigSystem.MeasureNoteNumber : 0 );
                    notePosEnd      = ( measure_no == measureNoEnd   ? ex % ConfigSystem.MeasureNoteNumber : ConfigSystem.MeasureNoteNumber - 1 );

                    for ( int note_pos = notePosStart; note_pos <= notePosEnd; note_pos++ )
                    {
                        if ( !measure.BpmLine.InfoStates.TryGetValue( note_pos, out var info ) )
                        {
                            continue;
                        }

                        rs.SelectBpm( info, true );
                    }
                }
            }
            #endregion
        }

        if ( rs.Count > 0 )
        {
            _EditResumeMng.ExcuteAndResume( rs );
        }
    }

    /// <summary>
    /// 範囲選択内のノート削除処理。
    /// 範囲選択のクリア処理を実施する為、実行前に _NoteRangeBef の設定が必要。
    /// </summary>
    private void RemoveNoteRange()
    {
        var rs = new ResumeMultiple();

        ClearSelectNoteRange( ref rs );

        foreach ( var info in Score.EditChannel.NoteInfoList.Values )
        {
            if ( info.Selected )
            {
                rs.RemoveNote( info );
            }
        }

        if ( DrawSet.IncludeBpm )
        { 
            foreach ( var info in Score.SysChannel.BpmInfoList.Values )
            {
                if ( info.Selected )
                {
                    rs.RemoveBpm( info );
                }
            }
        }

        if ( rs.Count > 0 )
        {
            _EditResumeMng.ExcuteAndResume( rs );
        }
    }

    /// <summary>
    /// 範囲選択の移動
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aStart">True:入力開始、False:入力途中/終了</param>
    private void EditMoveNoteRange( Point aMousePos, bool aStart )
    {
        var p = CalcNotePosition( aMousePos );

        if ( aStart )
        {
            _NoteRange.SetStartMovePos( p.X, p.Y );
        }
        else
        {
            _NoteRange.SetEndMovePos( p.X, p.Y );
        }
    }

    /// <summary>
    /// 範囲選択の移動処理
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    private void EditMoveNoteRange( Point aMousePos )
    {
        var rs = new ResumeMultiple();

        int a;
        int measure_no;
        int note_pos;
        int mv_x;
        int mv_y;

        #region NoteRange
        {
            EditMoveNoteRange( aMousePos, false );

            mv_x = _NoteRange.MoveNotePosX;
            mv_y = _NoteRange.MoveNotePosY;

            if ( mv_x == 0 && mv_y == 0 )
            {
                return;
            }

            rs.SelectNoteRange( ref _NoteRange, _NoteRangeBef );
        }
        #endregion

        #region Remove notes before and after the move
        {
            foreach ( var info_old in Score.EditChannel.NoteInfoList.Values )
            {
                if ( info_old.Selected )
                {
                    // Before
                    rs.RemoveNote( info_old );

                    // After
                    a = info_old.AbsoluteNotePos + mv_x;

                    measure_no  = a / ConfigSystem.MeasureNoteNumber;
                    note_pos    = a % ConfigSystem.MeasureNoteNumber;

                    var info_new = Score.EditChannel.GetNote( info_old.MidiMapKey, measure_no, note_pos );

                    if ( info_new != null )
                    {
                        rs.RemoveNote( info_new );
                    }
                }
            }
        }
        #endregion

        #region Add notes to move
        { 
            foreach ( var info_old in Score.EditChannel.NoteInfoList.Values )
            {
                if ( info_old.Selected )
                {
                    a = info_old.AbsoluteNotePos + mv_x;

                    measure_no  = a / ConfigSystem.MeasureNoteNumber;
                    note_pos    = a % ConfigSystem.MeasureNoteNumber;

                    int index = Score.EditMidiMapSet.GetDisplayMidiMapIndex( info_old.MidiMapKey );

                    if ( index == -1 )
                    {
                        continue;
                    }

                    index += mv_y;

                    int key = Score.EditMidiMapSet.GetDisplayMidiMapKey( index );

                    if ( key == -1 )
                    {
                        continue;
                    }

                    rs.AddNote( null, new( info_old.ChannelNo, key, measure_no, note_pos, info_old.Volume, info_old.NoteOn, info_old.NoteOff, true ) );
                }
            }
        }
        #endregion

        #region Remove bpms before and after the move
        {
            foreach ( var info_old in Score.SysChannel.BpmInfoList.Values )
            {
                if ( info_old.Selected )
                {
                    // Before
                    rs.RemoveBpm( info_old );

                    // After
                    a = info_old.AbsoluteNotePos + mv_x;

                    measure_no  = a / ConfigSystem.MeasureNoteNumber;
                    note_pos    = a % ConfigSystem.MeasureNoteNumber;

                    var info_new = Score.SysChannel.GetBpm( measure_no, note_pos );

                    if ( info_new != null )
                    {
                        rs.RemoveBpm( info_new );
                    }
                }
            }
        }
        #endregion

        if ( DrawSet.IncludeBpm )
        {
            #region Add bpms to move
            { 
                foreach ( var info_old in Score.SysChannel.BpmInfoList.Values )
                {
                    if ( info_old.Selected )
                    {
                        a = info_old.AbsoluteNotePos + mv_x;

                        measure_no  = a / ConfigSystem.MeasureNoteNumber;
                        note_pos    = a % ConfigSystem.MeasureNoteNumber;

                        rs.AddBpm( null, new( measure_no, note_pos, info_old.Bpm, true ) );
                    }
                }
            }
            #endregion
        }

        if ( rs.Count > 0 )
        {
            _EditResumeMng.ExcuteAndResume( rs );
        }
    }

    /// <summary>
    /// 範囲選択のコピー処理。
    /// 範囲選択のクリア処理を実施する為、実行前に _NoteRangeBef の設定が必要。
    /// </summary>
    private void CopyNoteRange()
    {
		_CopyNoteList.Clear();
        _CopyBpmList.Clear();

        _CopyNotePositionX = ConfigSystem.NoteCount;

		foreach ( var info in Score.EditChannel.NoteInfoList.Values )
        {
            if ( info.Selected )
            {
				if ( _CopyNotePositionX > info.AbsoluteNotePos )
				{
					_CopyNotePositionX = info.AbsoluteNotePos;
				}

				_CopyNoteList.Add( info.Clone() );
            }
        }

		foreach ( var info in Score.SysChannel.BpmInfoList.Values )
        {
            if ( info.Selected )
            {
				if ( _CopyNotePositionX > info.AbsoluteNotePos )
				{
					_CopyNotePositionX = info.AbsoluteNotePos;
				}

				_CopyBpmList.Add( info.Clone() );
            }
        }

        ClearSelectNoteRange();
    }

    /// <summary>
    /// NOTE/BPM貼付処理。
    /// 範囲選択のクリア処理を実施する為、実行前に _NoteRangeBef の設定が必要。
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    private void PasteNoteRange( Point aMousePos )
    {
        if ( !CheckScoreBodyArea( aMousePos ) )
        {
            return;
        }

		var pos = CalcNotePosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        var rs = new ResumeMultiple();

        ClearSelectNoteRange( ref rs );

        #region Add note
        foreach ( var info in _CopyNoteList )
        {
			int x = pos.X + info.AbsoluteNotePos - _CopyNotePositionX;

			int measure_no = x / ConfigSystem.MeasureNoteNumber;
			int note       = x % ConfigSystem.MeasureNoteNumber;

			var info_old = Score.EditChannel.GetNote( info.MidiMapKey, measure_no, note );
			var info_new = new InfoNote( Score.EditChannelNo, info.MidiMapKey, measure_no, note, info.Volume, info.NoteOn, info.NoteOff );

            rs.AddNote( info_old, info_new );
        }
        #endregion

        #region Add bpm

        if ( DrawSet.IncludeBpm )
        { 
            foreach ( var info in _CopyBpmList )
            {
				int x = pos.X + info.AbsoluteNotePos - _CopyNotePositionX;

				int measure_no = x / ConfigSystem.MeasureNoteNumber;
				int note       = x % ConfigSystem.MeasureNoteNumber;

				var info_old	= Score.SysChannel.GetBpm( measure_no, note );
				var info_new	= new InfoBpm( measure_no, note, info.Bpm );

                rs.AddBpm( info_old, info_new );
            }
        }

        #endregion

        if ( rs.Count > 0 )
        {
            _EditResumeMng.ExcuteAndResume( rs );
        }
    }

    /// <summary>
    /// ノート音量入力クリア
    /// </summary>
    private void ClearVolumeRange() => _VolumeRange.ClearPos();

    /// <summary>
    /// ノート音量入力の編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
    /// <param name="aStart">True:入力開始、False:入力途中/終了</param>
    private void EditVolumeRange( Point aMousePos, bool aStart )
    {
        if ( !CheckVolumeBodyArea( aMousePos ) )
        {
            return;
        }

		var pos = CalcNoteVolumePosition( aMousePos );

        if ( pos.X == -1 || pos.Y == -1 )
        {
            return;
        }

        if ( aStart )
        {
            _VolumeRange.SetStartPos( pos.X, pos.Y, DrawSet.VolumeEditSelect );
        }
        else
        {
            _VolumeRange.SetEndPos( pos.X, pos.Y );
        }
    }

    /// <summary>
    /// NOTE音量の編集
    /// </summary>
    /// <param name="aMousePos">マウス位置</param>
	private void EditNoteVolume( Point aMousePos )
	{
        EditVolumeRange( aMousePos, false );

        var rs = new ResumeMultiple();

        int sn = _VolumeRange.StartNotePosX;
        int en = _VolumeRange.EndNotePosX;

        {
            int measure_no;
            int note_pos;

            for ( int x = sn; x <= en; x++ )
            {
                measure_no  = x / ConfigSystem.MeasureNoteNumber;
                note_pos    = x % ConfigSystem.MeasureNoteNumber;

                var measure = Score.EditChannel.GetMeasure( measure_no );

                if ( measure == null )
                {
                    continue;
                }

				foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
				{
                    // 未選択の場合
                    if ( !( midiMap.Group?.Selected ?? false ) && !midiMap.Selected )
                    {
                        continue;
                    }

					if ( !measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var note_line ))
					{
                        continue;
                    }

                    if ( !note_line.InfoStates.TryGetValue( note_pos, out var info_old ) )
                    {
                        continue;
                    }

                    if ( info_old == null || !info_old.NoteOn )
                    {
                        continue;
                    }

                    var info_new = info_old.Clone();

                    switch ( _VolumeRange.EditType )
                    {
                        case ConfigEditer.VolumeEditType.VolumeUpDown:
                            info_new.Volume += _VolumeRange.VolumeList[ x - sn ];
                            break;
                        case ConfigEditer.VolumeEditType.VolumeIntonationHL:
                            info_new.Volume += _VolumeRange.VolumeList[ x - sn ]
                                * ( info_new.Volume < _VolumeRange.StartVolume ? -1 : 1 );
                            break;
                        case ConfigEditer.VolumeEditType.VolumeIntonationH:
                            info_new.Volume += _VolumeRange.VolumeList[ x - sn ]
                                * ( info_new.Volume >= _VolumeRange.StartVolume ? 1 : 0 );
                            break;
                        case ConfigEditer.VolumeEditType.VolumeIntonationL:
                            info_new.Volume += _VolumeRange.VolumeList[ x - sn ]
                                * ( info_new.Volume <= _VolumeRange.StartVolume ? 1 : 0 );
                            break;
                        default:
                            info_new.Volume  = _VolumeRange.VolumeList[ x - sn ];
                            break;
                    }

                    info_new.Volume = ConfigMedia.CheckMidiVolume( info_new.Volume );

                    rs.SetNoteVolume( info_old, info_new );
				}
            }
        }

        if ( rs.Count == 0 )
        {
            return;
        }

        _EditResumeMng.ExcuteAndResume( rs );

        #region ノート音量表示更新
        {
            int sm = sn / ConfigSystem.MeasureNoteNumber;
            int em = en / ConfigSystem.MeasureNoteNumber;

            for ( int m = sm; m <= em; m++ )
            {
                UpdateNoteVolumeMeasure( m );
            }
        }
        #endregion
    }

    #endregion

    #region Update

    /// <summary>
    /// フレーム処理
    /// </summary>
    public void OnMove()
	{
        #region Resume

        if ( DrawSet.UpdateRedoFlag )
        {
            _EditResumeMng.Redo();
            DrawSet.UpdateRedoFlag = false;
        }

        if ( DrawSet.UpdateUndoFlag )
        {
            _EditResumeMng.Undo();
            DrawSet.UpdateUndoFlag = false;
        }

        if ( DrawSet.UpdateResumeClearFlag )
        {
            _EditResumeMng.Clear();
            DrawSet.UpdateResumeClearFlag = false;
        }

        #endregion

        #region Score size

        if ( DrawSet.UpdateScoreLayoutFlag )
		{
            UpdateScore();
            DrawSet.UpdateScoreLayoutFlag = false;
        }

        #endregion

        #region Sheet potition

        if ( DrawSet.UpdateCameraFlag )
		{
            UpdateSheetPosition();
            DrawSet.UpdateCameraFlag = false;
		}

        #endregion

        #region Line

        if ( DrawSet.UpdateScoreLineFlag )
		{
            UpdateScoreLine();
            DrawSet.UpdateScoreLineFlag = false;
		}

        #endregion

        #region Score header

        if ( DrawSet.UpdateScoreHeaderFlag )
		{
            UpdateMidiMapHeader();
            DrawSet.UpdateScoreHeaderFlag = false;
		}

        #endregion

        #region Score All

        if ( DrawSet.UpdateScoreFlag )
        {
            UpdateBpmMeasure();
            UpdateNoteMeasure();
            UpdateNoteVolumeMeasure();
            UpdateNoteOnOff();

            DrawSet.UpdateScoreBpmMeasureNoList.Clear();
            DrawSet.UpdateScoreNoteMeasureNoList.Clear();
            DrawSet.UpdateScoreNoteVolumeMeasureNoList.Clear();

            DrawSet.UpdateScoreFlag             = false;
            DrawSet.UpdateScoreBpmFlag          = false;
            DrawSet.UpdateScoreNoteFlag         = false;
            DrawSet.UpdateScoreNoteVolumeFlag   = false;
        }

        #endregion

        #region Note

        if ( DrawSet.UpdateScoreNoteFlag )
		{
            foreach ( var measure_no in DrawSet.UpdateScoreNoteMeasureNoList )
            {
                UpdateNoteMeasure( measure_no );
                UpdateNoteVolumeMeasure( measure_no );
            }
            UpdateNoteOnOff();

            DrawSet.UpdateScoreNoteMeasureNoList.Clear();
            DrawSet.UpdateScoreNoteFlag = false;
        }

        #endregion

        #region Note Predict

        if ( DrawSet.UpdateScoreNotePredictFlag )
        {
            // TODO: 機械学習結果反映用：自動採譜を試してみるときに使うかも
            UpdateNotePredictListMeasure();
            DrawSet.UpdateScoreNotePredictFlag = false;
        }

        #endregion

        #region Bgm

        if ( DrawSet.UpdateScoreBgmFlag )
        {
            UpdateScaleBgm();
            DrawSet.UpdateScoreBgmFlag = false;
        }

        if ( DrawSet.UpdateScoreBgmScaleFlag )
        {
            UpdateScaleBgmBitmap();
        }

        #endregion

        #region Bpm

        if ( DrawSet.UpdateScoreBpmFlag )
        {
            foreach ( var measure_no in DrawSet.UpdateScoreBpmMeasureNoList )
            {
                UpdateBpmMeasure( measure_no );
            }
            DrawSet.UpdateScoreBpmMeasureNoList.Clear();
            DrawSet.UpdateScoreBpmFlag = false;
        }

        #endregion

        #region Note Volume

        if ( DrawSet.UpdateScoreNoteVolumeFlag )
        {
            foreach ( var measure_no in DrawSet.UpdateScoreNoteVolumeMeasureNoList )
            {
                UpdateNoteVolumeMeasure( measure_no );
            }
            DrawSet.UpdateScoreNoteVolumeMeasureNoList.Clear();
            DrawSet.UpdateScoreNoteVolumeFlag = false;
        }

        #endregion

        #region Clear range

        if ( DrawSet.UpdateClearRangeFlag )
        {
			_NoteRangeBef.Set( _NoteRange );

            ClearSelectNoteRange();

            DrawSet.UpdateClearRangeFlag = false;
        }

        #endregion
    }

    /// <summary>
    /// 表示範囲更新
    /// </summary>
    private void UpdateScore()
	{
		// Screen
        _ScreenSize.Width			= ActualSize.X;
        _ScreenSize.Height			= ActualSize.Y;

		// Infomation
        _InfoRange.X			    = 0;
        _InfoRange.Y			    = 0;
        _InfoRange.Width		    = DrawSet.HeaderWidthSize / 2;
        _InfoRange.Height		    = DrawSet.BpmHeightSize + DrawSet.MeasureNoHeightSize;

		// Bpm header
        _BpmHeadRange.X			    = _InfoRange.Right;
        _BpmHeadRange.Y			    = 0;
        _BpmHeadRange.Width		    = DrawSet.HeaderWidthSize / 2;
        _BpmHeadRange.Height		= DrawSet.BpmHeightSize;

		// Bpm body
        _BpmBodyRange.X			    = _BpmHeadRange.Right;
        _BpmBodyRange.Y			    = _BpmHeadRange.Top;
        _BpmBodyRange.Width		    = _ScreenSize.Width - _BpmHeadRange.Right > 0 ? _ScreenSize.Width - _BpmHeadRange.Right : 0 ;
        _BpmBodyRange.Height		= _BpmHeadRange.Height;

		// Measure number header
        _MeasureNoHeadRange.X		= _InfoRange.Right;
        _MeasureNoHeadRange.Y		= _BpmBodyRange.Bottom;
        _MeasureNoHeadRange.Width	= DrawSet.HeaderWidthSize / 2;
        _MeasureNoHeadRange.Height	= DrawSet.MeasureNoHeightSize;

		// Measure number body
        _MeasureNoBodyRange.X		= _MeasureNoHeadRange.Right;
        _MeasureNoBodyRange.Y		= _MeasureNoHeadRange.Top;
        _MeasureNoBodyRange.Width	= _ScreenSize.Width - _MeasureNoHeadRange.Right > 0 ? _ScreenSize.Width - _MeasureNoHeadRange.Right : 0 ;
        _MeasureNoBodyRange.Height	= _MeasureNoHeadRange.Height;

		// Score sheet header
        _ScoreHeadRange.X			= 0;
        _ScoreHeadRange.Y			= _MeasureNoBodyRange.Bottom;
        _ScoreHeadRange.Width		= DrawSet.HeaderWidthSize;
        _ScoreHeadRange.Height		= DrawSet.ScoreMaxHeight;

        // Score sheet body
        _ScoreBodyRange.X			= _ScoreHeadRange.Right;
        _ScoreBodyRange.Y			= _ScoreHeadRange.Top;
        _ScoreBodyRange.Width		= _ScreenSize.Width - _ScoreHeadRange.Right > 0 ? _ScreenSize.Width - _ScoreHeadRange.Right : 0 ;
        _ScoreBodyRange.Height		= _ScoreHeadRange.Height;

        // Volume header
        _VolumeHeadRange.X			= 0;
        _VolumeHeadRange.Y			= _ScreenSize.Height - DrawSet.VolumeHeightSize > 0 ? _ScreenSize.Height - DrawSet.VolumeHeightSize : 0 ;
        _VolumeHeadRange.Width		= DrawSet.HeaderWidthSize;
        _VolumeHeadRange.Height	    = DrawSet.VolumeHeightSize;

        // Volume body
        _VolumeBodyRange.X			= _VolumeHeadRange.Right;
        _VolumeBodyRange.Y			= _VolumeHeadRange.Top;
        _VolumeBodyRange.Width		= _ScreenSize.Width - _VolumeHeadRange.Right > 0 ? _ScreenSize.Width - _VolumeHeadRange.Right : 0 ;
        _VolumeBodyRange.Height	    = _VolumeHeadRange.Height;

        // スコア表示範囲
        {
            _ScoreViewHeight = _ScoreBodyRange._height;
                
            var h = _ScreenSize._height - _ScoreBodyRange._y - _VolumeBodyRange._height;

            if ( _ScoreViewHeight > h )
            {
                _ScoreViewHeight = h;
            }
        }

        // サポート線の列範囲を設定
        _SupportLine.SetNoteRect
            (
                _MeasureNoBodyRange._x,
                _MeasureNoBodyRange._y,
                DrawSet.NoteWidthSize,
                _MeasureNoBodyRange._height + _ScoreBodyRange._height
            );

        // ノート範囲（左上の位置を基準に設定）
        _NoteRange.SetNoteRect
            (
                _ScoreBodyRange._x,
                _ScoreBodyRange._y,
                DrawSet.NoteWidthSize,
                DrawSet.NoteHeightSize
            );

        // 音量範囲の底位置を再設定
        _VolumeRange.SetBottomPosition( _VolumeBodyRange._x, (float)_VolumeBodyRange.Bottom );

        // ノート音量の底位置を再設定
        foreach ( var list in _VolumeList.Values )
        {
            foreach ( var v in list )
            {
                v.Move( (float)_VolumeBodyRange.Bottom );
            }
        }
    }

    /// <summary>
    /// シート位置更新
    /// </summary>
    private void UpdateSheetPosition()
    {
		var body	    = _ScoreBodyRange;
        var view_h      = _ScoreViewHeight;
		var	note_pos    = DrawSet.NotePosition;

        #region 水平位置チェック
        {
            int w_cnt = ConfigSystem.NoteCount - (int)( body._width / DrawSet.NoteWidthSize );

            if ( note_pos.X > w_cnt )
            {
                note_pos.X = w_cnt;
            }

            if ( note_pos.X < 0 )
            {
                note_pos.X = 0;
            }
        }
        #endregion

        #region Vertical position check
        {
            int d_cnt = Score.EditMidiMapSet.DisplayMidiMapAllCount;
            int h_cnt = (int)( view_h / DrawSet.NoteHeightSize );

            if ( d_cnt <= h_cnt )
            {
                note_pos.Y = 0;
            }
            else if ( note_pos.Y > d_cnt - h_cnt )
            {
                note_pos.Y = d_cnt - h_cnt;
            }

            if ( note_pos.Y < 0 )
            {
                note_pos.Y = 0;
            }
        }
        #endregion

        // ノート位置設定
        DrawSet.NotePosition = new( note_pos.X, note_pos.Y );
    }

    /// <summary>
    /// 小節線表示更新
    /// </summary>
    private void UpdateScoreLine()
	{
        var body        = _ScoreBodyRange;
        var note_num    = ConfigSystem.MeasureNoteNumber;
        var note_width  = DrawSet.NoteWidthSize;

        #region 縦線
        {
            var pens = new FormatLine[]
                {
                    DrawSet.SheetMeasure001Line,
                    DrawSet.SheetMeasure004Line,
                    DrawSet.SheetMeasure008Line,
                    DrawSet.SheetMeasure016Line,
                    DrawSet.SheetMeasure032Line,
                    DrawSet.SheetMeasure064Line,
                    DrawSet.SheetMeasure128Line,
                };

            _MeasureLineList.Clear();

            var linesize = pens[ 0 ].LineSize == 0 ? 0 : 1;

            if ( linesize == 0 )
            {
                for ( int i = 1; i <= 6; i++ )
                {
                    if ( pens[ i ].LineSize != 0 )
                    {
                        linesize = (int)Math.Pow( 2, i + 1 );
                        break;
                    }
                }
            }

            if ( linesize != 0 )
            {
                FormatLine? pen = null;

                for ( int i = 0; i < note_num; i += linesize )
                {
                    for ( int x = 6, y = 1; x >= 0; x--, y *= 2 )
                    {
                        pen = pens[ x ];

                        if ( i % ( note_num / y ) == 0 && ( pen.LineSize != 0 ) )
                        {
                            break;
                        }
                    }

                    if ( pen == null )
                    {
                        continue;
                    }

                    _MeasureLineList.Add
                        (
                            new
                            (
                                body._x + note_width * i,
                                body._y,
                                0,
                                body._height,
                                pen
                            )
                        );

                    pen = null;
                }
            }
        }
        #endregion

        #region 横線
        {
            var pens = new FormatLine[]
                {
                    DrawSet.SheetStaffGroupLine,
                    DrawSet.SheetStaffMidiMapLine
                };

            int index = -1;

            for ( int i = 0; i <= 1; i++ )
            {
            	if ( pens[ i ].LineSize != 0 )
            	{
            		index = i;
                    break;
            	}
            }

            _StaffLineList.Clear();

            if ( index != -1 )
            {
                var x1  = body._x;
                var x2  = body._width;
                var y   = body._y;

                for ( int i = 0; i < Score.EditMidiMapSet.DisplayGroupCount; i++ )
                {
                    #region MidiMapGroupの上線

      				_StaffLineList.Add( new( x1, y, x2, 0, pens[ index ] ) );

                	y += DrawSet.NoteHeightSize;

                    #endregion

                    #region MidiMapの上線

                    int midiMapCount = Score.EditMidiMapSet.DisplayMidiMapCountByGroup[ i ];

                    var pen = pens[ 1 ];

                    if ( pen.LineSize != 0 )
                    {
                		for ( int j = 1; j < midiMapCount; j++ )
                		{
                			_StaffLineList.Add( new( x1, y, x2, 0, pen ) );

                			y += DrawSet.NoteHeightSize;
                		}
                    }
                    else
                    {
                		y += DrawSet.NoteHeightSize * ( midiMapCount - 1 );
                    }

                    #endregion
                }

                // 一番下の線
                _StaffLineList.Add( new( x1, y, x2, 0, pens[ index ] ) );
            }
        }
        #endregion
    }

    /// <summary>
    /// MidiMapGroup/MidiMapヘッダ表示更新
    /// </summary>
    private void UpdateMidiMapHeader()
	{
        var head = _ScoreHeadRange;

        _HeaderList.Clear();

        var tmp_y = head._y;

        // 描画範囲の左上の座標基準
        float x;
        float y;
        float w;
        float h;

        #region MidiMapGroup
        {
            x = head._x;
            y = head._y;
            w = head._width / 2;

            for ( int index = 0; index < Score.EditMidiMapSet.DisplayGroupCount; index++ )
            {
                h = DrawSet.NoteHeightSize * Score.EditMidiMapSet.DisplayMidiMapCountByGroup[ index ];

                var group = Score.EditMidiMapSet.GetDisplayMidiMapGroup( index );

                if ( group != null )
                {
                    // ヘッダ情報追加
                    _HeaderList.Add( new( x, y, w, h, group ) );
                }

                y += h;
            }
        }
        #endregion

        #region MidiMap
        {
            x = head._x + w;
			y = tmp_y;
			w = head._width - w;
			h = DrawSet.NoteHeightSize;

			foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
			{
                // ノート描画用の書式を登録
                if ( !_MidiMapNoteFormatList.ContainsKey( midiMap.MidiMapKey ) )
                {
                    // TODO: 線の色とか情報追加が必要
                    var formatRect = new FormatRect
                    {
                        BackColor = midiMap.Color
                    };

                    _MidiMapNoteFormatList.Add( midiMap.MidiMapKey, formatRect );
                }
                else
                {
                    _MidiMapNoteFormatList[ midiMap.MidiMapKey ].BackColor = midiMap.Color;
                }

                // ヘッダ情報追加
                _HeaderList.Add( new( x, y, w, h, midiMap ) );

				y += h;
			}
        }
        #endregion
    }

    /// <summary>
    /// 小節ノート表示更新
    /// </summary>
    private void UpdateNoteMeasure()
    {
        for ( int measure_no = 0; measure_no <= ConfigSystem.MeasureMaxNumber; measure_no++ )
        {
            UpdateNoteMeasure( measure_no );
        }
    }

    /// <summary>
    /// 小節ノート表示更新
    /// </summary>
    /// <param name="aMeasureNo">小節番号</param>
    private void UpdateNoteMeasure( int aMeasureNo )
    {
        #region Clear
        {
            if ( _NoteList.TryGetValue( aMeasureNo, out var nList ) )
            {
                nList.Clear();
                _NoteList.Remove( aMeasureNo );
            }
        }
        #endregion

        var measure = Score.EditChannel.GetMeasure( aMeasureNo );

        if ( measure == null )
        {
            return;
        }

        var body_s = _ScoreBodyRange;

        // 描画範囲の左上の座標を基準に算出
        var note_rect = new Rect
            ( 
                0, 
                body_s._y,
                DrawSet.NoteWidthSize,
                DrawSet.NoteHeightSize 
            );

        #region Note
		{
			foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
			{
				if ( measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var measure_line ) )
				{
					foreach ( var info in measure_line.InfoStates.Values )
					{
						note_rect.X = body_s.X + info.NotePos * note_rect.Width - note_rect.Height / 2;

						if ( !_NoteList.TryGetValue( aMeasureNo, out var lst ) )
						{
							lst = new();
						}
						lst.Add
                            (
							    new
								(
									note_rect._x,
								    note_rect._y,
								    note_rect._height,
								    note_rect._height,
                                    info,
					                _MidiMapNoteFormatList[ midiMap.MidiMapKey ]
                                )
					        );

						_NoteList[ aMeasureNo ] = lst;
					}
				}
				note_rect.Y += note_rect.Height;
			}
		}
        #endregion

        #region Sort
        {
            if ( _NoteList.TryGetValue( aMeasureNo, out var lst ) )
            {
                lst.Sort();
            }
        }
        #endregion
    }

    /// <summary>
    /// ノートON/OFFの関連付け更新
    /// </summary>
    private void UpdateNoteOnOff()
    {
        // TODO: NOTE-OFF の実装方法が微妙、Drumのみであれば問題ないが

        foreach ( var item in _NoteList.Values )
        {
            foreach ( var note in item )
            { 
                if ( note?.InfoNote?.NoteOn ?? false )
                {
                    var distanceToNextNoteOff = Score.EditChannel.GetNotePosDistanceToNextNoteOff( note.InfoNote );
                    note.NoteLength = distanceToNextNoteOff == 0 ? 0 : distanceToNextNoteOff * DrawSet.NoteWidthSize ;
                }
            }
        }

#if false
        var befNoteOff = new Dictionary<int, DmsItemNote>();

        for ( int measure_no = Config.System.MeasureMaxNumber; measure_no >= 0; measure_no-- )
        {
            if ( !_NoteList.TryGetValue( measure_no, out var lst ) )
            {
                continue;
            }

            lst.Reverse();

            foreach ( var item in lst )
            {
                item.NoteLength = 0;
                item.NoteOnItem = null;

                if ( item.InfoNote == null )
                {
                    continue;
                }

                if ( item.InfoNote.NoteOn )
                {
                    if ( befNoteOff.TryGetValue( item.InfoNote.MidiMapKey, out var bef_item ) && bef_item.InfoNote != null )
                    {
                        item.NoteLength = ( bef_item.InfoNote.MeasureNo * DrawSet.MeasureSize + bef_item.NoteRect.Left ) 
                                        - ( item.InfoNote.MeasureNo     * DrawSet.MeasureSize + item.NoteRect.Right );

                        bef_item.NoteOnItem = item;

                        befNoteOff.Remove( item.InfoNote.MidiMapKey );
                    }
                }

                if ( item.InfoNote.NoteOff )
                {
                    befNoteOff[ item.InfoNote.MidiMapKey ] = item;
                }
            }

            lst.Sort();
        }

        befNoteOff.Clear();
#endif
    }

    /// <summary>
    /// 小節ノート予測表示更新
    /// </summary>
    private void UpdateNotePredictListMeasure()
    {
        for ( int measure_no = 0; measure_no <= ConfigSystem.MeasureMaxNumber; measure_no++ )
        {
            UpdateNotePredictListMeasure( measure_no );
        }
    }

    /// <summary>
    /// 小節ノート予測表示更新
    /// </summary>
    /// <param name="aMeasureNo">小節番号</param>
    private void UpdateNotePredictListMeasure( int aMeasureNo )
    {
        #region Clear
        {
            if ( _NotePredictList.TryGetValue( aMeasureNo, out var nList ) )
            {
                nList.Clear();
                _NotePredictList.Remove( aMeasureNo );
            }
        }
        #endregion

        var measure = ScorePredict.EditChannel.GetMeasure( aMeasureNo );

        if ( measure == null )
        {
            return;
        }

        var body_s = _ScoreBodyRange;

        // 描画範囲の左上の座標を基準に算出
        var note_rect = new Rect
            ( 
                0, 
                body_s._y,
                DrawSet.NoteWidthSize,
                DrawSet.NoteHeightSize 
            );

        #region Note
		{
			foreach ( var midiMap in ScorePredict.EditMidiMapSet.DisplayMidiMaps )
			{
				if ( measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var measure_line ) )
				{
					foreach ( var info in measure_line.InfoStates.Values )
					{
						note_rect.X = body_s.X + info.NotePos * note_rect.Width - note_rect.Height / 2;

						if ( !_NotePredictList.TryGetValue( aMeasureNo, out var lst ) )
						{
							lst = new();
						}
						lst.Add
                            (
							    new
								(
									note_rect._x,
								    note_rect._y,
								    note_rect._height,
								    note_rect._height,
                                    info,
					                _MidiMapNoteFormatList[ midiMap.MidiMapKey ]
                                )
					        );

						_NotePredictList[ aMeasureNo ] = lst;
					}
				}
				note_rect.Y += note_rect.Height;
			}
		}
        #endregion

        #region Sort
        {
            if ( _NotePredictList.TryGetValue( aMeasureNo, out var lst ) )
            {
                lst.Sort();
            }
        }
        #endregion
    }

    /// <summary>
    /// 小節ノート音量表示更新
    /// </summary>
    private void UpdateNoteVolumeMeasure()
    {
        for ( int measure_no = 0; measure_no <= ConfigSystem.MeasureMaxNumber; measure_no++ )
        {
            UpdateNoteVolumeMeasure( measure_no );
        }

        DrawSet.VolumeDisplay = Score.EditMidiMapSet.IsSelectMidiMap();
    }

    /// <summary>
    /// 小節ノート音量表示更新
    /// </summary>
    /// <param name="aMeasureNo">小節番号</param>
    private void UpdateNoteVolumeMeasure( int aMeasureNo )
    {
        #region Clear
        {
            if ( _VolumeList.TryGetValue( aMeasureNo, out var nList ) )
            {
                nList.Clear();
                _VolumeList.Remove( aMeasureNo );
            }
        }
        #endregion

        var measure = Score.EditChannel.GetMeasure( aMeasureNo );

        if ( measure == null )
        {
            return;
        }

        var body = _VolumeBodyRange;
        var n_w  = DrawSet.NoteWidthSize;

        #region Note
		{
			foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
			{
                // 未選択の場合
                if ( !( midiMap.Group?.Selected ?? false ) && !midiMap.Selected )
                {
                    continue;
                }

				if ( !measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var measure_line ) )
				{
                    continue;
                }

				foreach ( var info in measure_line.InfoStates.Values )
				{
                    if ( !info.NoteOn )
                    {
                        continue;
                    }

					if ( !_VolumeList.TryGetValue( aMeasureNo, out var lst ) )
					{
						lst = new();
					}
					lst.Add
                        (
                            new
                            (
                                body._x + info.NotePos * n_w,
                                (float)body.Bottom,
                                info,
                                _MidiMapNoteFormatList[ midiMap.MidiMapKey ]
                            )
                        );

                    _VolumeList[ aMeasureNo ] = lst;
				}
			}
		}
        #endregion
    }

    /// <summary>
    /// 小節BPM表示更新
    /// </summary>
    private void UpdateBpmMeasure()
    {
        for ( int measure_no = 0; measure_no <= ConfigSystem.MeasureMaxNumber; measure_no++ )
        {
            UpdateBpmMeasure( measure_no );
        }
    }

    /// <summary>
    /// 小節BPM表示更新
    /// </summary>
    /// <param name="aMeasureNo">小節番号</param>
    private void UpdateBpmMeasure( int aMeasureNo )
    {
        #region Clear
        {
            if ( _BpmList.TryGetValue( aMeasureNo, out var nList ) )
            {
                nList.Clear();
                _BpmList.Remove( aMeasureNo );
            }
        }
        #endregion

        var body = _BpmBodyRange;

        // 描画範囲の左上の座標基準
        var note_rect = new Rect
            ( 
                0, 
                body._y,
                DrawSet.NoteWidthSize,
                DrawSet.BpmHeightSize 
            );

        #region Bpm
        {
            var measure = Score.SysChannel.GetMeasure( aMeasureNo );

            if ( measure == null )
            {
                return;
            }

            var bpm_line = measure.BpmLine;

            if ( bpm_line == null )
            {
				return;
			}

            foreach ( var info in bpm_line.InfoStates.Values )
            {
                note_rect.X = body.X + info.NotePos * DrawSet.NoteWidthSize - DrawSet.NoteWidthSize / 2;

                if ( !_BpmList.TryGetValue( aMeasureNo, out var lst ) )
                {
                    lst = new();
                }

                lst.Add
                    ( 
                        new
                        (
                            note_rect._x,
                            note_rect._y,
                            note_rect._width,
                            note_rect._height,
                            info
                        )
                    );

                _BpmList[ aMeasureNo ] = lst;
            }
        }
        #endregion

        #region Sort
        {
            if ( _BpmList.TryGetValue( aMeasureNo, out var lst ) )
            {
                lst.Sort();
            }
        }
        #endregion
    }

    /// <summary>
    /// BGM音階表示更新
    /// </summary>
    private void UpdateScaleBgm()
    {
        try
        {
            DrawSet.UpdateScoreBgmScaleFlag = false;

            _ScaleBgm?.Dispose();
            _ScaleBgm = null;

            if (Score.BgmFilePath.IsExistFile )
            { 
                _ScaleBgm = new NAudioData(Score.BgmFilePath, true );
            }

            DrawSet.UpdateScoreBgmScaleFlag = true;
        }
        catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
    }

    /// <summary>
    /// BGM音階表示画像更新
    /// </summary>
    private async void UpdateScaleBgmBitmap()
    {
        await Task.Run
            ( 
                () =>
                    { 
                        if ( _ScaleBgm == null )
                        {
                            _ScaleBitmap?.Dispose();
                            _ScaleBitmap = null;

                            DrawSet.UpdateScoreBgmScaleFlag = false;
                            return;
                        }

                        if ( !_ScaleBgm.IsEnableFFT() )
                        {
                            return;
                        }

                        DrawSet.UpdateScoreBgmScaleFlag = false;

                        try
                        {
                            _ScaleBitmap = CreateScaleBitmap();
                        }
                        catch ( Exception e )
			            {
                            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
			            }
                    } 
            );
    }

    /// <summary>
    /// 音階別周波数表示用Bitmap作成
    /// </summary>
    /// <returns>音階別周波数表示用Bitmap</returns>
    private CanvasBitmap? CreateScaleBitmap()
    {
        if ( !( _ScaleBgm?.IsEnableFFT() ?? false ) )
        {
            return null;
        }

        // TODO: WaveForm 何か案を思いつけば対応

        var t_level = ConfigScale.VolumeLevelTop;
        var h_level = ConfigScale.VolumeLevelHigh;
        var m_level = ConfigScale.VolumeLevelMid;
        var l_level = ConfigScale.VolumeLevelLow;

        if ( t_level - l_level <= 0F )
        {
            return null;
        }

		if ( !Score.EditMidiMapSet.DisplayMidiMaps
                .Where( item => ConfigScale.GetScaleListIndex( item.Scale ) != -1 ).Any() )
        {
            return null;
        }

        var bgm_time        = _ScaleBgm.GetDuration();
        var note_pos_x_last = DmsControl.SearchPosition( bgm_time.TotalSeconds + Score.BgmPlaybackStartPosition );
        var note_rect       = new Rect( 0, 0, 1, 1 );

        var body = new Rect
            (
                0,
                0,
                note_rect.Width  * note_pos_x_last,
                note_rect.Height * Score.EditMidiMapSet.DisplayMidiMapAllCount
            );

        var device = CanvasDevice.GetSharedDevice();

        var bmp = default( CanvasBitmap );

        var offscreen = new CanvasRenderTarget
            (
                device,
                body._width,
                body._height, 
                96
            );

        using var g = offscreen.CreateDrawingSession();

        // 背景色塗りつぶし
        g.Clear(DrawSet.SheetColor.Color );

        #region 周波数
        {
            for ( int fft_offset = 0; fft_offset < _ScaleBgm.FFTBufferLength0; fft_offset++ )
            {
                var note_pos_time   = _ScaleBgm.GetFFTTime( fft_offset );
                var note_pos_x      = DmsControl.SearchPosition( note_pos_time + Score.BgmPlaybackStartPosition );

                var fft             = _ScaleBgm.GetFFTBuffer( fft_offset );
                var hzPerOne        = (float)_ScaleBgm.GetSampleRate() / ( fft.Count * _ScaleBgm.Channels );

                var midimap_index = -1;

			    foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
			    {
                    midimap_index++;

                    var index = ConfigScale.GetScaleListIndex( midiMap.Scale );

                    if ( index == -1 )
                    {
                        continue;
                    }

                    var item = ConfigScale.ScaleList[ index ];

                    var fft_index = (int)( item.Hz / hzPerOne ) * _ScaleBgm.Channels;

                    if ( fft_index >= fft.Count )
                    {
                        continue;
                    }

                    var db = fft[ fft_index ];
                    for ( int x = 1; x < _ScaleBgm.Channels; x++ )
                    {
                        db = Math.Max( db, fft[ fft_index + x ] );
                    }

                    db = ( db - l_level ) * ( 1F / ( t_level - l_level ) );

                    if ( db < 0F )
                    {
                        db = 0F;
                    }
                    else if ( db > 1F )
                    {
                        db = 1F;
                    }

                    byte alpha   = 255;//155 + (int)( 100 * db );
                    byte red     = (byte)( db >= h_level ? 255 * db : 0 );
                    byte green   = (byte)( db >= m_level ? 255 * db : 0 );
                    byte blue    = (byte)( db >= l_level ? 255 * db : 0 );
                    //var red     = t_level > db && db >= h_level ? (int)( 255 * db ) : 0;
                    //var green   = h_level > db && db >= m_level ? (int)( 255 * db ) : 0;
                    //var blue    = m_level > db && db >= l_level ? (int)( 255 * db ) : 0;

                    if ( db > 0F )
                    {
                        note_rect.X = body.X + note_rect.Width  * note_pos_x;
                        note_rect.Y = body.Y + note_rect.Height * midimap_index;

                        g.FillRectangle
                            ( 
                                note_rect,
                                Color.FromArgb( alpha, red, green, blue )  
                            );
                    }
                }
            }
        }
        #endregion

        return bmp;
    }

    #endregion

    #region Draw

    /// <summary>
    /// キャンバス描画更新
    /// </summary>
    public void Refresh() => _EditerCanvas.Invalidate();

    /// <summary>
    /// Win2D アンロード処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void UserControl_Unloaded( object sender, RoutedEventArgs args )
    {
        try
        {
            // Win2D アンロード
            //_EditerCanvas.RemoveFromVisualTree();
            //_EditerCanvas = null;
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    /// <summary>
    /// 描画
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EditerCanvas_Draw( CanvasControl sender, CanvasDrawEventArgs args )
    {
        try
        {
            OnMove();

            var body            = _ScoreBodyRange;
            var note_pos        = DrawSet.NotePosition;
            var sheet_pos       = new Point( note_pos.X * DrawSet.NoteWidthSize, note_pos.Y * DrawSet.NoteHeightSize );
            var measure_size    = DrawSet.MeasureSize;
            int measure_start   = note_pos.X / ConfigSystem.MeasureNoteNumber;
            int measure_end     = (int)( ( note_pos.X + body._width / DrawSet.NoteWidthSize ) / ConfigSystem.MeasureNoteNumber + 1 );

            #region WaveForm
            {
                // TODO: WaveForm 何か案を思いつけば対応

                // 画像表示
                if ( ConfigScale.WaveFormOn && _ScaleBitmap != null)
                {
                    // 描画範囲
                    var destRect = new Rect
                        (
                            body.X,
                            body.Y,
                            body.Width,
                            body.Height
                        );

                    // 切取範囲
                    var srcRect = new Rect
                        (
                            sheet_pos.X     / DrawSet.NoteWidthSize,
                            sheet_pos.Y     / DrawSet.NoteHeightSize,
                            destRect.Width  / DrawSet.NoteWidthSize,
                            destRect.Height / DrawSet.NoteHeightSize
                        );

                    var t_level = ConfigScale.VolumeLevelTop;
                    var h_level = ConfigScale.VolumeLevelHigh;
                    var m_level = ConfigScale.VolumeLevelMid;
                    var l_level = ConfigScale.VolumeLevelLow;

                    // 色補正
                    var matrix = new Matrix4x4
                    {
                        M11 = ConfigScale.SensitivityLevel >= h_level / 4.0F ? ConfigScale.SensitivityLevel : 0F,
                        M22 = ConfigScale.SensitivityLevel >= m_level / 3.0F ? ConfigScale.SensitivityLevel : 0F,
                        M33 = ConfigScale.SensitivityLevel >= l_level / 2.0F ? ConfigScale.SensitivityLevel : 0F,
                        M44 = 1F,
                    };

                    // 描画
                    args.DrawingSession.DrawImage
                        (
                            _ScaleBitmap,
                            destRect,
                            srcRect,
                            1.0F,
                            CanvasImageInterpolation.NearestNeighbor,
                            matrix
                        );
                }
            }
            #endregion

            #region Cursor horizontal line
            {
                if ( _NoteCursorPosition.Y - note_pos.Y >= 0 )
                {
                    var rect = new Rect
                        (
                            body._x,
                            body._y + ( _NoteCursorPosition.Y - note_pos.Y ) * DrawSet.NoteHeightSize,
                            body._width,
                            DrawSet.NoteHeightSize
                        );

                    args.DrawingSession.FillRectangle
                        (
                            rect,
                            DrawSet.SheetCursorHorizonLine.Color
                        );
                }
            }
            #endregion

            #region Bar line
            {
                float diff_x;

                for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                {
                    diff_x = measure_size * measure_no - sheet_pos._x;

                    foreach ( var line in _MeasureLineList )
                    {
                        line.Draw( args.DrawingSession, diff_x, -sheet_pos._y );
                    }
                }
            }
            #endregion

            #region Staff
            {
                foreach ( var line in _StaffLineList )
                {
                    line.Draw( args.DrawingSession, 0, -sheet_pos._y );
                }
            }
            #endregion

            #region Support line
            {
                _SupportLine.Draw( args.DrawingSession, measure_start, measure_end, note_pos.X, -sheet_pos._y );
            }
            #endregion

            #region Note
            {
                float diff_x;

                for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                {
                    if ( !_NoteList.TryGetValue( measure_no, out var notes ) )
                    {
                        continue;
                    }

                    diff_x = measure_size * measure_no - sheet_pos._x;

                    foreach ( var note in notes )
                    {
                        note.Draw( sender, args.DrawingSession, diff_x, -sheet_pos._y );
                    }
                }
            }
            #endregion

            #region Note predict
            {
                float diff_x;

                for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                {
                    if ( !_NotePredictList.TryGetValue( measure_no, out var notes ) )
                    {
                        continue;
                    }

                    diff_x = measure_size * measure_no - sheet_pos._x;

                    foreach ( var note in notes )
                    {
                        note.DrawPredict( args.DrawingSession, diff_x, -sheet_pos._y );
                    }
                }
            }
            #endregion

            #region Bpm
            {
                #region body
                {
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _BpmBodyRange,
                            DrawSet.BpmBodyRect,
                            ""
                        );

                    float diff_x;

                    for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                    {
                        if ( !_BpmList.TryGetValue( measure_no, out var bpms ) )
                        {
                            continue;
                        }

                        diff_x = measure_size * measure_no - sheet_pos._x;

                        foreach ( var bpm in bpms )
                        {
                            bpm.Draw( args.DrawingSession, diff_x, 0 );
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region NoteRange
            {
                _NoteRange.Draw( args.DrawingSession, note_pos );
            }
            #endregion

            #region MidiMapGroup / MidiMap
            {
                foreach ( var obj in _HeaderList )
                {
                    obj.Draw( args.DrawingSession, 0, -sheet_pos._y );
                }
            }
            #endregion

            #region Bpm
            {
                #region header
                {
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _BpmHeadRange,
                            DrawSet.BpmHeadRect,
                            $"{_Resource.GetString( "LabelBpm" )}"
                        );
                }
                #endregion
            }
            #endregion

            #region Volume
            if ( DrawSet.VolumeDisplay && _VolumeList.Count != 0 )
            {
                #region body
                {
                    // 背景描画
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _VolumeBodyRange,
                            DrawSet.VolumeBodyRect,
                            ""
                        );

                    // 音量描画
                    float diff_x;

                    for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                    {
                        if ( !_VolumeList.TryGetValue( measure_no, out var notes ) )
                        {
                            continue;
                        }

                        diff_x = measure_size * measure_no - sheet_pos._x;

                        foreach ( var note in notes )
                        {
                            note.Draw( args.DrawingSession, diff_x );
                        }
                    }
                }
                #endregion

                #region 音量入力線
                {
                    _VolumeRange.DrawRange( args.DrawingSession, note_pos.X );
                }
                #endregion

                #region Header
                {
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _VolumeHeadRange,
                            DrawSet.VolumeHeadRect,
                            $"{_Resource.GetString( "LabelVolume" )}"
                        );
                }
                #endregion
            }
            #endregion

            #region Measure number
            {
                #region body
                {
                    // 背景色
                    args.DrawingSession.FillRectangle
                        (
                            _MeasureNoBodyRange,
                            DrawSet.MeasureNoBodyRect.BackColor
                        );

                    var rect = new Rect
                        (
                            _MeasureNoBodyRange.X,
                            _MeasureNoBodyRange.Y,
                            measure_size,
                            _MeasureNoBodyRange.Height
                        );

                    for ( int measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                    {
                        rect.X = _MeasureNoBodyRange.X + measure_size * measure_no - sheet_pos.X;

                        // 外枠
                        args.DrawingSession.DrawRectangle
                            (
                                rect,
                                DrawSet.MeasureNoBodyRect.LineColor,
                                DrawSet.MeasureNoBodyRect.LineSize
                            );

                        rect.X += 5;

                        // テキスト
                        args.DrawingSession.DrawText
                            (
                                String.Format( "{0:000}", measure_no ),
                                rect,
                                DrawSet.MeasureNoBodyRect.TextColor,
                                DrawSet.MeasureNoBodyRect.TextFormat
                            );
                    }
                }
                #endregion

                #region header
                {
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _MeasureNoHeadRange,
                            DrawSet.MeasureNoHeadRect,
                            $"{_Resource.GetString( "LabelMeasureNo" )}"
                        );
                }
                #endregion
            }
            #endregion

            #region Cursor vertical line
            {
                if ( _NoteCursorPosition.X - note_pos.X >= 0 )
                {
                    var x = body._x + ( _NoteCursorPosition.X - note_pos.X ) * DrawSet.NoteWidthSize;

                    args.DrawingSession.DrawLine
                        (
                            x,
                            body._y,
                            x,
                            (float)body.Bottom,
                            DrawSet.SheetCursorVerticleLine.LineColor,
                            DrawSet.SheetCursorVerticleLine.LineSize
                        );
                }
            }
            #endregion

            #region Info
            {
                #region header
                {
                    XamlHelper.FormatRectDraw
                        (
                            args.DrawingSession,
                            _InfoRange,
                            DrawSet.InfoHeaderRect,
                            $"{DrawSet.NotePosition.X % ConfigSystem.MeasureNoteNumber}:{DrawSet.NotePosition.Y}"
                        );
                }
                #endregion
            }
            #endregion
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    /// <summary>
    /// リサイズイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void UserControl_SizeChanged( object sender, SizeChangedEventArgs args )
    {
        try
        {
            Config.EventEditerPanelResize();
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
    }

    #endregion
}
