using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ExamplePlugin.Commands;

/// <summary>
/// Cosole command definition interface.
/// </summary>
interface IConCommand {
    /// <summary>
    /// The command to be input by the user, e.g.: <c>sign_in</c>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Help text to display (TODO: where?)
    /// </summary>
    string HelpText { get; }

    /// <summary>
    /// A function called by the game's runtime, performs the designated
    /// command line action.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    void Execute(ConCommandArgs args);
}

/// <summary>
/// A console command, that is exclusively used to communicate with the
/// race server.
/// </summary>
/// <typeparam name="I">
/// Type of the request payload
/// </typeparam>
/// <typeparam name="O">
/// Type of the response payload. Should be able to be converted from
/// a JSON string by <see cref="JsonUtility.FromJson{T}(string)"/>
/// </typeparam>
internal abstract class ApiConCommand<I, O> : IConCommand
    where I: IIntoJson
{
    public abstract string Name { get; }
    public abstract string HelpText { get; }
    public void Execute(ConCommandArgs args) {
        ICollection<string> errors = new List<string>();
        TryParseArgs(args, ref errors, out I payload);

        if (errors.Count > 0) {
            foreach (var err in errors) {
                Debug.LogError(err);
            }
            return;
        }

        switch (Method) {
            case HttpMethod.GET:
                throw new NotImplementedException();
            case HttpMethod.POST:
                var req = Api.PostRequest<I, O>(
                    Endpoint, 
                    payload, 
                    (req) => OnError(payload, req), 
                    (res, req) => OnDone(payload, res, req)
                );
                Api.Send(req);
                break;
        }
    }

    /// <summary>
    /// The API endpoint this command's request should be sent to.
    /// </summary>
    protected abstract Endpoint Endpoint { get; }

    /// <summary>
    /// The HTTP method that should be used for this command's request.
    /// </summary>
    protected abstract HttpMethod Method { get; }

    /// <summary>
    /// Try to construct the request's payload from the command's arguments.
    /// Any errors that occur during the operation should be added to the 
    /// provided <paramref name="errors"/> collection. If there are any, these
    /// will be displayed to the user and the request will not be sent. Otherwise,
    /// the <paramref name="parsed"/> object will be used as the request's payload.
    /// </summary>
    /// <param name="args">
    /// The raw command line arguments.
    /// </param>
    /// <param name="errors">
    /// A collection to insert validation errors.
    /// </param>
    /// <param name="parsed">
    /// The command line arguments, parsed as the request payload.
    /// </param>
    protected abstract void TryParseArgs(
        ConCommandArgs args, 
        ref ICollection<string> errors, 
        out I parsed
    );

    /// <summary>
    /// A callback that runs when the request failed.
    /// </summary>
    /// <param name="payload">
    /// The payload sent as the request body/query parameters.
    /// </param>
    /// <param name="req">
    /// The web request object.
    /// </param>
    protected virtual void OnError(I payload, UnityWebRequest req) { Api.LogError(req); }

    /// <summary>
    /// A callback that runs when the request is successful.
    /// </summary>
    /// <param name="payload">
    /// The payload sent as the request body/query parameters.
    /// </param>
    /// <param name="res">
    /// The API response.
    /// </param>
    /// <param name="req">
    /// The web request object.
    /// </param>
    protected virtual void OnDone(I payload, O res, UnityWebRequest req) { }      
}
