#Enigma knacken 
from Enigma import *
import itertools
import time
import numpy as np
import multiprocessing
from itertools import product


def coincidence_index(text):
    n = len(text)
    freqs = np.bincount([ord(c.lower()) - ord('a') for c in text if c.isalpha()], minlength=26)
    return np.sum(freqs * (freqs - 1)) / (n * (n - 1))

#erzeugt einen String
buchstaben = "abcdefghijklmnopqrstuvwxyz"

#erzeugt eine Liste von allen möglichen Permutationen von den Walzen 1 bis 5
lst_walzen = list(itertools.permutations(range(1,6),3))

#erzeugt eine Liste mit den drei Umkehrwalzen 
lst_umkehrwalzen = [1 , 2, 3]

#erzeugt eine Liste von allen möglichen Kombinationen von 3 Buchstaben von a bis z als String
moeglichkeiten_list = [''.join(p) for p in product(buchstaben, repeat= 3)]

#erzeugt eine Liste von Zahlen von 0 bis 26
lst_zahlen  = list(range(27))

#erzeugt alle möglichen Ringkombinationen 
lst_ringkombinationen = list(itertools.combinations_with_replacement(lst_zahlen, 3))

#erzeugt die Liste für die Ergebnisse
ergebnisse = []

#erzeugt Liste für die Walzenkombis nach dem Programm
lst_ergebnissewalzenkombi = []

#erzeugt Liste für die Walzenkombis vor dem Programm
walzenkombi = []

#erzeugt Liste die die Walzen mit deren Position und der Umkehrwalze enthält
walzen_Einstellung = [] 


#erzeugt eine Liste mit allen Permutationen der Walzenkombi_positionen, dies beinhaltet die Nummern der Walzen und Walzen Postionen
#lst_walzenkombi_position = list(product(lst_walzen, moeglichkeiten_list, lst_umkehrwalzen))
lst_walzenkombi_position = list(product(lst_walzen, moeglichkeiten_list, ))
print(lst_walzenkombi_position)
print(len(lst_walzenkombi_position))


#erzeugt ein Dictionary mit allen Permutationen der Walzenkombi_positionen, dies beinhaltet die Nummern der Walzen und Walzen Postionen
dic_walzenkombi_postion = {}

for index, element in enumerate(lst_walzenkombi_position):
    dic_walzenkombi_postion[index] = element

print(dic_walzenkombi_postion[1])


#erzeugt die Aufforderung zur Texteingabe im Terminal und verändert alle Großbuchstaben zu Kleinbuchstaben
text = input('Texteingabe:').lower()
startzeit = time.time()

enigma = Enigma_maschine()

#for walzenkombi_position in lst_walzenkombi_position:




for walzenkombi_position, value in dic_walzenkombi_postion.items():
    a, b = value
    enigma.einstellungen(a, b, 1, [1,1,1],  "")
    umgewandelter_text = umwandeln(enigma, text)
    lst_ergebnissewalzenkombi.append(umgewandelter_text)
    #indexwert = coincidence_index(umgewandelter_text)
    #lst_ergebnissewalzenkombi.append(indexwert)
    #print(dic_walzenkombi_postion[walzenkombi_position])


#enigma.einstellungen((1,1,1), "cpt", 2, [1,1,1],  "")

#print(indexwert)

#print(lst_walzen)

#print(moeglichkeiten_list)


#for walzenkombi_position in dic_walzenkombi_postion:
   # a, b, c = dic_walzenkombi_postion[walzenkombi_position]
   # print(a)
    #print(b)
   # print(c)

endzeit = time.time()

#print(lst_ergebnissewalzenkombi)


dauer = endzeit - startzeit

print("Dauer:", dauer, "Sekunden")