namespace HelloFSharp

open Xamarin.Forms
open Xamarin.Forms.Xaml

type MainPage() = 
    inherit ContentPage()
    
    do base.LoadFromXaml(typeof<MainPage>) |> ignore