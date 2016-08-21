﻿(*
Copyright (c) 2013-2015 FSharp.ViewModule Team

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*)

namespace ViewModule

open System.ComponentModel
open System.Windows.Input

open Microsoft.FSharp.Quotations

/// <summary>Extension of ICommand with a public method to fire the CanExecuteChanged event</summary>
/// <remarks>This type should provide a constructor which accepts an Execute (obj -> unit) and CanExecute (obj -> bool) function</remarks>
type INotifyCommand =
    inherit ICommand 
    
    /// Trigger the CanExecuteChanged event for this specific ICommand
    abstract RaiseCanExecuteChanged : unit -> unit

/// <summary>Extension of INotifyCommand with a public property to supply a CancellationToken.</summary>
/// <remarks>This allows the command to change the token for subsequent usages if required</remarks>
type IAsyncNotifyCommand =
    inherit INotifyCommand

    abstract member CancellationToken : System.Threading.CancellationToken with get, set

/// Interface used to explicitly raise PropertyChanged
type IRaisePropertyChanged =
    inherit INotifyPropertyChanged

    abstract RaisePropertyChanged : propertyName : string -> unit

/// Results of a validation for the member of a type or an entity.  errorKey is a string identifier unique per "error case"
type ValidationState =
    | PropertyValidation of propertyName : string * error : string list
    | EntityValidation of error : string list

/// Used to track validation errors
type IValidationTracker =
    /// Set a validation result tied to and generated by a specific property
    abstract SetPropertyValidationState : ValidationState -> unit
    /// Set a validation result generated by the entity level validation
    abstract SetEntityValidationResult : ValidationState -> unit
    /// Clear all errors from the validation
    abstract ClearErrors : unit -> unit
    /// Force a re-computation of a specific property's validation results
    abstract Revalidate : string -> unit
    /// Add a watcher for a property to validate
    abstract AddPropertyValidationWatcher : string -> (unit -> string list) -> unit

/// This interface is a tag for implementation of extension methods. Do not implement externally.
type IDependencyTracker =
    interface end
    
    // DependencyTracker is the only implementation of IDependencyTracker and implements the following members:
    //
    // AddPropertyDependenciesI : string * string list -> unit
    // AddPropertyDependencyI : string * string -> unit
    // AddCommandDependencyI : INotifyCommand * string -> unit
    
    // The following can be implemented as extension methods by casting to DependencyTracker for the 
    // F# API in the ViewModule.FSharp namespace:
    //
    // AddPropertyDependencies : string * string list -> unit
    // AddPropertyDependency : string * string -> unit
    // AddPropertyDependency : Expr * Expr -> unit
    // AddPropertyDependencies : Expr * Expr list -> unit
    //
    //
    // The following can be implemented via extension methods by casting to DependencyTracker for the
    // C# API in the ViewModule.CSharp namespace:
    //
    // AddPropertyDepenency : property : string  * dependency : string -> unit
    // AddPropertyDepenencies : property : string  * dependency : string * [<ParamArray>] rest : string array -> unit
    //
    //
    // In addition, the following extension methods are implemented in the namespace ViewModule.CSharp.Expressions
    // to support LINQ Expression trees for C# versions which do not have nameof:
    //
    // AddPropertyDepenency : property : Expression<Func<obj>> * dependency : Expression<Func<obj>> -> unit
    // AddPropertyDepenencies : property : Expression<Func<obj>> * dependency : Expression<Func<obj>> * [<ParamArray>] rest : Expression<Func<obj>> array -> unit

/// <summary>Extension of INotifyPropertyChanged with a public method to fire the PropertyChanged event</summary>
/// <remarks>This type should provide a constructor which accepts no arguments, and one which accepts a Model</remarks>
type IViewModel =
    inherit INotifyPropertyChanged
    inherit IRaisePropertyChanged
    inherit INotifyDataErrorInfo
    
    /// Value used to notify view that an asynchronous operation is executing
    abstract OperationExecuting : bool with get, set

    /// Handles management of dependencies for all computed properties 
    /// as well as ICommand dependencies
    abstract DependencyTracker : IDependencyTracker


