﻿using System;
using System.Diagnostics;
using CommunityToolkit.WinUI.Notifications;
using DrumMidiEditorApp.pGeneralFunction.pLog;
using DrumMidiEditorApp.pGeneralFunction.pUtil;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace DrumMidiEditorApp.pView.pDebug;

public sealed partial class PageDebugShell : Page
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public PageDebugShell()
    {
		// 初期化
		InitializeComponent();
	}

	private void OpenButton_Click( object sender, RoutedEventArgs args )
	{
        Process.Start( "EXPLORER.EXE", $"{AppDirectory.AppBaseDirectory}" );
    }

    private void ToastShowButton_Click( object sender, RoutedEventArgs args )
    {
        try
        {
            // トースト通知からのアプリアクティブ化
            //ToastNotificationManagerCompat.OnActivated += toastArgs =>
            //{
            //    var args = ToastArguments.Parse( toastArgs.Argument );

            //    // Obtain any user input (text boxes, menu selections) from the notification
            //    var userInput = toastArgs.UserInput;

            //    // 値をUIへ反映する場合、Dispatcherの処理が必要
            //};

            // トーストコンテンツを構成後、通知
            new ToastContentBuilder()
                // ---------------------------------------------------------
                // Launch:ユーザーがトーストをクリックしたときにアプリに渡される引数を定義
                // ---------------------------------------------------------
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)

                // ---------------------------------------------------------
                // Visual:トーストの視覚的な部分
                // ---------------------------------------------------------

                // テキスト要素は３つまで追加可能。１行目がメインで、２，３行目がサブ情報
                .AddText( "トースト　テスト通知", hintMaxLines: 2 )
                .AddText( "説明要素：歩きスマホは" )
                .AddText( "説明要素：止めましょう"  )

                // インライン画像
                .AddInlineImage( new Uri( "https://avatars.githubusercontent.com/u/97685486?v=4" ) )

                // アプリ ロゴの上書き：Windows 11では無効?（アプリロゴは Attribute 領域に表示）
                //.AddAppLogoOverride(new Uri("ms-appdata:///local/Andrew.jpg"), ToastGenericAppLogoCrop.Circle)

                // ヒーロー イメージ：Attribute領域の上にイメージを表示
                .AddHeroImage( new Uri( "https://user-images.githubusercontent.com/97685486/182838106-50765a8a-814a-42c6-9714-23a23284b593.png" ) )

                /**
                 * URI指定パターン
                 *   - http://          http/https リモート Web画像 ファイルサイズの制限あり
                 *   - ms-appx:///      
                 *   - ms-appdata:///   
                 */

                // カスタム タイムスタンプ：通知時刻をアプリ側から指定可能
                .AddCustomTimeStamp( new DateTime( 2017, 04, 15, 19, 45, 00, DateTimeKind.Utc ) )

                // 進捗状況バー
                .AddVisualChild
                    (
                        new AdaptiveProgressBar()
                        {
                            Title               = "進捗バー",
                            Value               = new BindableProgressBarValue( "progressValue" ),
                            ValueStringOverride = new BindableString( "progressValueString" ),
                            Status              = new BindableString( "progressStatus" )
                        }
                    )

                // ---------------------------------------------------------
                // Action:トーストの対話的な部分 (入力やアクションなど) 
                // ---------------------------------------------------------

                // テキスト入力
                .AddInputTextBox( "tbReply", placeHolderContent: "Type a response" )

                // 選択入力
                .AddToastInput
                    (
                        new ToastSelectionBox( "time" )
                        {
                            DefaultSelectionBoxItemId = "lunch",
                            Items =
                            {
                                new ToastSelectionBoxItem( "breakfast"  , "Breakfast"   ),
                                new ToastSelectionBoxItem( "lunch"      , "Lunch"       ),
                                new ToastSelectionBoxItem( "dinner"     , "Dinner"      ),
                            }
                        }
                    )

                // 再通知時間入力
                .AddToastInput
                    (
                        new ToastSelectionBox( "snoozeTime" )
                        {
                            DefaultSelectionBoxItemId = "15",
                            Items =
                            {
                                new( "5"      , "5 minutes"     ),
                                new( "15"     , "15 minutes"    ),
                                new( "60"     , "1 hour"        ),
                                new( "240"    , "4 hours"       ),
                                new( "1440"   , "1 day"         )
                            }
                        })

                // ボタン
                .AddButton
                    (
                        new ToastButton()
                            .SetContent("Reply")
                            .SetTextBoxId( "tbReply" )
                            .AddArgument("action", "reply")
                            .SetBackgroundActivation()
                    )
                .AddButton
                    (
                        new ToastButton()
                            .SetContent("Like")
                            .AddArgument("action", "like")
                            .SetBackgroundActivation()
                    )
                //.AddButton
                //    ( 
                //        new ToastButton()
                //            .SetContent("View")
                //            .AddArgument("action", "viewImage")
                //            .AddArgument("imageUrl", image.ToString() ) 
                //    )

                // [一時停止(再通知)]ボタン
                .AddButton
                    (
                        new ToastButtonSnooze() 
                        { 
                            SelectionBoxId = "snoozeTime",
                        }
                    )

                // [無視]ボタン
                .AddButton( new ToastButtonDismiss() )

                // シナリオ
                .SetToastScenario( ToastScenario.Reminder )         // リマインダー
                //.SetToastScenario( ToastScenario.Alarm )          // アラーム：トースト通知に少なくとも 1 つのボタンを指定する必要あり
                //.SetToastScenario( ToastScenario.IncomingCall )   // 着信呼び出し
                //.SetToastScenario( ToastScenario.******** )       // 重要な通知：未実装

                // ---------------------------------------------------------
                // Audio:トーストがユーザーに表示されるときに再生されるオーディオ
                //
                // [既定のサウンド一覧]
                // https://docs.microsoft.com/ja-jp/uwp/schemas/tiles/toastschema/element-audio#attributes-and-elements
                // ---------------------------------------------------------
                .AddAudio( new Uri( "ms-appx:///Sound.mp3" ) )

                // ---------------------------------------------------------
                // Show
                // ---------------------------------------------------------
                .Show
                (
                    ( toast ) =>
                    {
                        toast.Tag               = "TagName";
                        toast.Group             = "GroupName";
                        toast.ExpirationTime    = DateTime.Now.AddMinutes( 3 );     // 有効期限

                        toast.Data = new NotificationData();
                        toast.Data.Values[ "progressValue" ]            = "0.6";
                        toast.Data.Values[ "progressValueString" ]      = "15/26 songs";
                        toast.Data.Values[ "progressStatus" ]           = "Downloading...";
                        toast.Data.SequenceNumber                       = 0;      // 0(常に更新する)、1-(シーケンス位置指定)
                    }
                ); 
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    private void ToastUpdateButton_Click( object sender, RoutedEventArgs args )
    {
        try
        {
            var data = new NotificationData
            {
                SequenceNumber = 0,      // 0(常に更新する)
            };

            data.Values[ "progressValue" ]          = "1.0";
            data.Values[ "progressValueString" ]    = "26/26 songs";
            data.Values[ "progressStatus" ]         = "Finished";

            ToastNotificationManager.CreateToastNotifier().Update( data, "TagName", "GroupName" );
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    private void ToastClearButton_Click( object sender, RoutedEventArgs args )
    {
        try
        {
            // 全てのトースト履歴をクリア
            ToastNotificationManagerCompat.History.Clear();

            // 対象グループのトースト履歴を削除
            ToastNotificationManagerCompat.History.RemoveGroup( "GroupName" );

            // 対象タグのトースト履歴を削除
            ToastNotificationManagerCompat.History.Remove( "TagName" );

            // 対象タグ・グループに一致するトースト履歴を削除
            ToastNotificationManagerCompat.History.Remove( "TagName", "GroupName" );
        }
        catch ( Exception e )
        {
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

}