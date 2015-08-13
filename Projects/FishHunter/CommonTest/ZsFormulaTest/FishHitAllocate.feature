Feature: FishHitAllocate
	In order to 依擊中的魚數量，分配武器威力
	As a math 分配威力表
	I want to be 得到命中機率


    
Background: 
	Given 威力分配表
    | HitNumber | Hit1 | Hit2 | Hit3 | Hit4 |
    | 1         | 1000 | 0    | 0    | 0    |
    | 2         | 800  | 200  | 0    | 0    |
    | 3         | 750  | 150  | 100  | 0    |
    | 4         | 700  | 150  | 100  | 50   |


Scenario: 擊中1隻魚
	When 擊中數量是1
	Then 取出資料為
    | HitNumber | Hit1 | Hit2 | Hit3 | Hit4 |
    | 1         | 1000 | 0    | 0    | 0    |

Scenario: 擊中2隻魚
	When 擊中數量是2
	Then 取出資料為
    | HitNumber | Hit1 | Hit2 | Hit3 | Hit4 |
    | 2         | 800  | 200  | 0    | 0    |

Scenario: 擊中3隻魚
	When 擊中數量是3
	Then 取出資料為
    | HitNumber | Hit1 | Hit2 | Hit3 | Hit4 |
    | 3         | 750  | 150  | 100  | 0    |

Scenario: 擊中4隻魚以上
	When 擊中數量是5
	Then 取出資料為
    | HitNumber | Hit1 | Hit2 | Hit3 | Hit4 |
    | 4         | 700  | 150  | 100  | 50   |   