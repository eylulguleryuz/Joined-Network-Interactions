Canteen
Components: Canteen (server), baker, eater.
There are 24 hours in a night. From 8H to 16H baker components generate random positive amounts of new food portions baked. 
Generated amounts are accumulated in the server. From 17H to 7H baker components do nothing. From 11H to 18H eater components 
generated random positive amounts of food portios eaten. Generated amounts are removed from the server while also increasing 
profit and reputation in proportion to the amount removed. From 18H to 10H eater components do nothing. If there is not enough 
food to satisfy the eaters, reputation decreases in proportion to the amount of missing food portions. After 19H all food 
portions left are thrown away and profit is reduced proportionally to the amount of food lost. If canteen maintains negative 
profit or reputation for 2 subsequent nights, it gets closed for 1 night. While canteen is closed both bakers and eaters do 
nothing. Afterwards, canteen is reset to a fresh state and simulation continues. Hours follow each other in their natural order.
