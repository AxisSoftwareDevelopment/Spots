# Welcome to Cloud Functions for Firebase for Python!
# To get started, simply uncomment the below code or create your own.
# Deploy with `firebase deploy`

from firebase_functions import https_fn
from firebase_admin import initialize_app, messaging

app = initialize_app()

@https_fn.on_call()
def TRIGGER_NOTIFICATION(request: https_fn.CallableRequest) -> https_fn.Response:
    print("\n\t<----- Starting Cloud Function ----->")
    try:
        param_Type = request.data["type"]
        param_Topic = request.data["topic"]
        param_FCMTokens = request.data["fcm_tokens"]
        param_Title = request.data["title"]
        param_Body = request.data["body"]
        print(f"\n-> Type: [{param_Type}]\n-> Topic: {param_Topic}\n-> Tokens: {str(param_FCMTokens)}\n-> Title: {param_Title}\n-> Body: {param_Body}")

        notification = messaging.Notification (
            title=param_Title,
            body=param_Body)
            #image="" )

        msgs = []

        if param_Type == "TOKENS":
            if len(param_FCMTokens) < 1:
                print("No Tokens passed.")
                return
            print(f"There are [{len(param_FCMTokens)}] tokens to send notifications to.")
            msgs = [ messaging.Message(token=token, notification=notification) for token in param_FCMTokens ]
        else:
            msgs = [ messaging.Message(topic=param_Topic, notification=notification) ]

        batch_response: messaging.BatchResponse = messaging.send_each(msgs)

        if batch_response.failure_count < 1:
            print("Messages sent sucessfully")
        else:
            print(f"[{str(batch_response.failure_count)}] messages failed.")
    except Exception as ex:
        print(str(ex))
    print("\n\t<----- Finished Cloud Function ----->")
    