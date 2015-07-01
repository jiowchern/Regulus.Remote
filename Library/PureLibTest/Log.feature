#language: zh-TW

功能: Log
	In Order to 寫入訊息
	As a 程式物件
	I Want to 輸出到Dummy物件

 場景: 當訊息為Message時，印出[Info]Message
	 假設 Log寫入資料是"Message" 	 
	 當 寫入到LogInfo
	 那麼 輸出為"[Info]Message"

場景: 當訊息為Message時，印出[Debug]Message
	 假設 Log寫入資料是"Message" 	 
	 當 寫入到LogDebug
	 那麼 輸出為"[Debug]Message"
@mytag
 
