﻿namespace Todo

open System
open System.IO
open MonoTouch.UIKit
open MonoTouch.Foundation
open Xamarin.Forms

open Tamarin

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    let mutable window: UIWindow = null
    let mutable eventLoop: IDisposable = null

    let conn = 
        let sqliteFilename = "TodoSQLite.db3";
        let documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
        let libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
        let path = Path.Combine(libraryPath, sqliteFilename);

        // This is where we copy in the prepopulated database
        if not <| File.Exists path 
        then 
            File.Copy (sqliteFilename, path)

        let plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS()
        let conn = new SQLite.Net.SQLiteConnection(plat, path)

        // Return the database connection 
        conn

    override this.FinishedLaunching (app, options) =
        Forms.Init ()
        window <- new UIWindow (UIScreen.MainScreen.Bounds)

        let model, view, controller = TodoListModel(), TodoListView(), TodoListController( conn)
        let mvc = Mvc(model, view, controller)
        eventLoop <- mvc.Start()

        window.RootViewController <- NavigationPage(view.Root).CreateViewController ()
        window.MakeKeyAndVisible ()
        true

    override this.WillTerminate _ =
        eventLoop.Dispose()

module Main =
    [<EntryPoint>]
    let main args = 
        UIApplication.Main (args, null, "AppDelegate")
        0

        