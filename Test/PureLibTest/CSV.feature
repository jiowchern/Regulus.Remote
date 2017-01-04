Feature: CSV
	In order to 解析CSV格式資料
	As a 程式物件
	I want to 把CSV資料轉成對應物件

@mytag
# field1,field2,field3
# 1,2,3
# 4,5,6
# TypeA typeA = Read<TypeA>("field1,field2,field3 1,2,3 4,5,6");
Scenario: 讀取串流資料
	Given 資料是field1,field2,field3 1,2,3 4,5,6
	And 段落符號為" "
	And 分格符號為","
	When 執行解析
	Then 結果為 
	|field1 |field2|field3|
	|1|2|3|
	|4|5|6|
