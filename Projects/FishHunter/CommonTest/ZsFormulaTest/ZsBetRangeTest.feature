Feature: ZsBetRangeTestSteps
	In order to 計算魚場賭注
	As a math 賭注分配公式
	I want to be 根據公式得到5個押分區間(最小~最大)


Background: 
Given buffer資料是
    | Id | Rate |
    | 1  | 0.1  |
    | 2  | 0.25 |
    | 3  | 0.5  |
    | 4  | 0.75 |
    | 5  | 1.0  |


Scenario: 取得魚場押注buffer等級1
    
    When 最大押分是1000
    And 玩家押分是100
    
    Then 取得的Buffer是
    | Id | Rate |
    | 1  | 0.1  |
    

Scenario: 取得魚場押注buffer等級2

    When 最大押分是1000
    And 玩家押分是250
    
    Then 取得的Buffer是
    | Id | Rate |
    | 2  | 0.25  |

Scenario: 取得魚場押注buffer等級3

    When 最大押分是1000
    And 玩家押分是500
    
    Then 取得的Buffer是
    | Id | Rate |
    | 3  | 0.5  |

Scenario: 取得魚場押注buffer等級4

    When 最大押分是1000
    And 玩家押分是750
    
    Then 取得的Buffer是
    | Id | Rate |
    | 4  | 0.75  |

    
Scenario: 取得魚場押注buffer等級5

    When 最大押分是1000
    And 玩家押分是1000
    
    Then 取得的Buffer是
    | Id | Rate |
    | 5  | 1.0  |