using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pGeneralFunction.pLog;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;

namespace DrumMidiEditorApp.pView.pEditer.pScore;

public sealed partial class PageScore : Page
{
	/// <summary>
	/// スコア設定
	/// </summary>
	private ConfigScore DrawSet => Config.Score;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public PageScore()
    {
        InitializeComponent();

		// NumberBox の入力書式設定
		_NoteHeightNumberBox.NumberFormatter
			= XamlHelper.CreateNumberFormatter( 1, 1, 0.1 );
		_NoteWidthNumberBox.NumberFormatter
			= XamlHelper.CreateNumberFormatter( 1, 1, 0.1 );
	}

	/// <summary>
	/// スコア更新フラグ設定
	/// </summary>
	private void UpdateScore()
	{
		Config.EventUpdateScoreTab();

		_ScorePanel.Refresh();
	}

	/// <summary>
	/// 共通：トグル切替
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	private void ToggleSwitch_Toggled( object sender, RoutedEventArgs args )
    {
		try
		{
			UpdateScore();
		}
		catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

	/// <summary>
	/// 共通：数値変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
    private void NumberBox_ValueChanged( NumberBox sender, NumberBoxValueChangedEventArgs args )
    {
		try
		{
			// 必須入力チェック
			if ( !XamlHelper.NumberBox_RequiredInputValidation( sender, args ) )
            {
				return;
            }

			UpdateScore();
		}
		catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }
}
