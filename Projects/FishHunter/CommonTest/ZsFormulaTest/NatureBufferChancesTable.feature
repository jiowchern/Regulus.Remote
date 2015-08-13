Feature: NatureBufferChancesTable
	In order to 計算自然buffer
	As a math 自然buffer表
	I want to be 根據公式得到buffer區間(最小~最大)

@mytag

Background: 
Given buffer資料是
	| Id | Rate |
	| -3 | -500 |
	| -2 | -150 |
	| -1 | -100 |
	| 0  | -50  | 
	| 1  | -30  |
	| 2  | 0    |
	| 3  | 20   |
	| 4  | 50   |
	| 5  | 100  |
	| 6  | 150  |

    
Scenario: 取得自然buffer區間
    When 基數是-3

    Then 取得的Buffer是
    | Id | Rate |
	| -3 | -500 |
	

