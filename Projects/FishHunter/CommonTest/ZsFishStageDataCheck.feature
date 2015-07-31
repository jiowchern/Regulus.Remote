Feature: ZsFishStageDataCheck
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: 取得0號魚場資料

	Given 魚場資料表是
	| StageId | Name | SizeType | BaseOdds | GameRate | MaxBet | NowBaseOdds | BaseChgOddsCnt |
	| 0       | 魚場1  | SMALL    | 100      | 995      | 1000   | 0           | 0              |
	| 1       | 魚場2  | MEDIUM   | 200      | 995      | 1000   | 0           | 0              |
	| 2       | 魚場3  | MEDIUM   | 200      | 995      | 1000   | 0           | 0              |
	| 3       | 魚場4  | LARGE    | 300      | 995      | 1000   | 0           | 0              |
	
	When 當輸入魚場id是 0
	
    Then 取得的魚場資料是
    | StageId | Name | SizeType | BaseOdds | GameRate | MaxBet | NowBaseOdds | BaseChgOddsCnt |
    | 0       | 魚場1  | SMALL    | 100      | 995      | 1000   | 0           | 0              |


	