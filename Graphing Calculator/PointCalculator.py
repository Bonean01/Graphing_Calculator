from math import *

def calculate_point(formula, x):
    y_point = eval(formula)
    if y_point < 800:
        return y_point

def formula_decoder(formula):
    formula_storing(formula)

def formula_storing(formula):
    position = 0
    with open("FormulaStorage.txt", "r+") as file:
        # counting number of characters in the file.
        for line in file:
            position += 1
            for _ in line:
                position += 1
        # setting the position in the file to be the last character.
        file.seek(position)
        file.writelines(formula+"\n")