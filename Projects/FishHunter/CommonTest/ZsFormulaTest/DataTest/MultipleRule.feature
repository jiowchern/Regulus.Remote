Feature: MultipleRule
	In order to 取得翻倍的倍數
	As a math 翻倍表
	I want to be 回傳取得的倍數
    
Background: 
Given 倍數表是
    | Multiple | Value |
    | 1        | 1     |
    | 2        | 2     |
    | 3        | 3     |
    | 5        | 5     |
    | 10       | 10    |


Scenario: 取得倍數1
    When 輸入倍數表id是1
	
    Then 得到的倍數值是1


Scenario: 取得倍數2
    When 輸入倍數表id是2
	
    Then 得到的倍數值是2

Scenario: 取得倍數3
    When 輸入倍數表id是3
	
    Then 得到的倍數值是3

Scenario: 取得倍數5
    When 輸入倍數表id是5
	
    Then 得到的倍數值是5

Scenario: 取得倍數10
    When 輸入倍數表id是10
	
    Then 得到的倍數值是10


