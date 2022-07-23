﻿using Microsoft.UI.Xaml.Controls;
using System;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pGeneralFunction.pLog;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;

namespace DrumMidiEditorApp.pView.pEditer.pEdit;

public sealed partial class PageInputBpm : Page
{
	/// <summary>
	/// メディア設定
	/// </summary>
	private ConfigMedia ConfigMedia => Config.Media;

	/// <summary>
	/// BPM入力値
	/// </summary>
	public double Bpm { get; set; } = Config.System.DefaultBpm;

	/// <summary>
	/// コンストラクタ
	/// </summary>
    public PageInputBpm()
    {
        InitializeComponent();

		// NumberBox の入力書式設定
		_BpmNumberBox.NumberFormatter
			= XamlHelper.CreateNumberFormatter( 1, 2, 1 );
	}

	/// <summary>
	/// ＢＰＭ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	private void BpmNumberBox_ValueChanged( NumberBox sender, NumberBoxValueChangedEventArgs args )
    {
		try
		{
			// 必須入力チェック
			if ( !XamlHelper.NumberBox_RequiredInputValidation( sender, args ) )
            {
				return;
            }
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
    }
}