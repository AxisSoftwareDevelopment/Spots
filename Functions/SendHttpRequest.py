import requests
contents = requests.post("http://127.0.0.1:5001/spotsv01/us-central1/TRIGGER_NOTIFICATION", json={"data":{"type":"TOKENS","title":"Table Invitation","body":"TestUser invites you to join the TestTable table.","fcm_tokens":["d3ZhSEW5ROqj95Jo2Yvjse:APA91bG8gcTHTftb2_P0hfSdHq-EDp_blCtYaNfIR2BA_sG5gLZguxkIFJjXvX7vHD-YS1NCq1PoBoYxXlOfgFf9FeJ8zIzwqraZgyP4K_7zTR4TUpppwdL_rCnPTLYCPN05YH6hkznw"],"topic":"null"}})
#contents = requests.post("http://127.0.0.1:5001/spotsv01/us-central1/TRIGGER_NOTIFICATION", json={"data":{"TestVar":"TestVal"}})
print(contents)