namespace HelloFSharp

open Xamarin.Forms
open Xamarin.Forms.Xaml

type App() = 
    inherit Application()

    do base.LoadFromXaml(typeof<App>) |> ignore
       base.MainPage <- new HelloFSharp.MainPage()
    
    override this.OnStart() = ()  // Handle when your app starts
    
    override this.OnSleep() = () // Handle when your app sleeps
    
    override this.OnResume() = () // Handle when your app resumes