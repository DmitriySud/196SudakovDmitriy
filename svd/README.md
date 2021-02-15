# SVD algorithm
Программа, реализующая SVD(Singular value decomposition) разложение прямоугольной матрицы. 
Считывает данные из input.txt, выводит в стандартный поток матрицы leftSingularVectors, rightSingularVectors и массив singularValues
 
## Пример
Матрица 
```
1 2 3
2 3 4
3 4 5
```
Вывод : 
``` 
Left singular vectors
  -0.3851   0.8277   0.4082
  -0.5595   0.1424  -0.8165
  -0.7339  -0.5428   0.4082
  
Right singular vectors
  -0.3851  -0.8277   0.4082
  -0.5595  -0.1424  -0.8165
  -0.7339   0.5428   0.4082

Singular values
  9.6235   0.6235   0.0000
```

Вычисления производятся в соответствии с [простым итеративным алгоритмом](http://www.machinelearning.ru/wiki/index.php?title=Простой_итерационный_алгоритм_сингулярного_разложения)
