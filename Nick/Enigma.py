#Enigma

from collections import deque

def zahlenwandler(text):
    
    return[ord(c)-97 for c in text] 


#standartwalze='abcdefghijklmnopqrstuvwxyz'

#standartwalze=[deque(zahlenwandler(standartwalze))]

standartwalze = deque(range(26))

#print(standartwalze)

walzen=['ekmflgdqvzntowyhxuspaibrcj', #I    
        'ajdksiruxblhwtmcqgznpyfvoe', #II
        'bdfhjlcprtxvznyeiwgakmusqo', #III
        'esovpzjayquirhxlnftgkdcmwb', #IV
        'vzbrgityupsdnhlxawmjqofeck'  #V
        ]

walzen=[deque(zahlenwandler(z)) for z in walzen]

#print(walzen)

ukw=['ejmzalyxvbwfcrquontspikhgd', #A
     'yruhqsldpxngokmiebfzcwvjat', #B
     'fvpjiaoyedrzxwgctkuqsbnmhl'  #C
    ]

ukw=[zahlenwandler(z) for z in ukw]

uebertragskerbe= "q e v j z"

uebertragskerbe=[zahlenwandler(z) for z in uebertragskerbe.split()]

#print(uebertragskerbe)

class Walze():
    def __init__(self, nummer, walzen_position, ring_position):
        self.walzen_position = walzen_position
        self.ring_position = ring_position
        self.walzenteil_rechts = walzen[-1+nummer].copy()   
        self.walzenteil_links = standartwalze.copy()
        self.uebertragskerbe = uebertragskerbe[-1+nummer]
        self.einstellungen()
    
    def einstellungen(self):
        verschiebung = self.ring_position-self.walzen_position
        self.walzenteil_links.rotate(verschiebung)
        self.walzenteil_rechts.rotate(verschiebung)
        self.uebertragskerbe = [(kerbe-self.ring_position) % 26 for kerbe in self.uebertragskerbe]

    def prüfen(self):
        for nummer in self.walzenteil_links:
            print(chr(nummer+97), end='')
        print()
        

        for nummer in self.walzenteil_rechts:
            print(chr(nummer+97), end='')
        print()
        

        for nummer in self.uebertragskerbe:
            print(chr(nummer+97), end='') 
        print()
    
    def wrotieren(self):
        self.walzenteil_links.rotate(-1)
        self.walzenteil_rechts.rotate(-1)

    def schalten(self):
        return self.walzenteil_links[0] in self.uebertragskerbe
    

class Enigma_maschine():
    def __init__(self):
        self.walzen = []
        self.ukw = []
        self.stecker = {}
    
    def einstellungen(self, nummer_walzen, walzen_position, nummer_ukw, ring_position,  steckerpaare):
        self.walzen = []
        for i, nummer in enumerate(nummer_walzen):
            walzenpostion = ord(walzen_position[i])-97
            ringposition = ring_position[i]-1
            self.walzen.append(Walze(nummer, walzenpostion, ringposition))
        self.ukw = ukw[nummer_ukw-1]
        for a,b in steckerpaare.split():
                self.stecker[ord(a)-97] = ord(b)-97
                self.stecker[ord(b)-97] = ord(a)-97
    

    def rotieren(self):
        erstewalze, zweitewalze, drittewalze = self.walzen
        
        if zweitewalze.schalten():
            zweitewalze.wrotieren()
            erstewalze.wrotieren()
        elif drittewalze.schalten():
            zweitewalze.wrotieren()
        drittewalze.wrotieren()
    

#def umwandeln(e, text):
     #   umgewandelter_text = ""
     #   for c in text:
          #  c = ord(c)-97
           # if c < 0 or c> 26: continue
           # e.rotieren()
           # c = e.stecker.get(c,c)
           # for w in reversed(e.walzen):
            #    c = w.walzenteil_rechts[c]
            #    c = w.walzenteil_links.index(c)
           # c = e.ukw[c]
           # for w in e.walzen:
            #    c = w.walzenteil_links[c]
            #    c = w.walzenteil_rechts.index(c)
           # c = e.stecker.get(c,c)
           # umgewandelter_text += chr(c+97)
       # return umgewandelter_text



import numpy as np

#def umwandeln(e, text):
  #  alphabet = np.arange(26)
  #  stecker = np.array([e.stecker.get(i, i) for i in alphabet])
  #  umgewandelter_text = ""
   # for c in text:
      #  c = ord(c)-97
      #  if c < 0 or c > 25: continue
       # e.rotieren()
       # c = stecker[c]
       # for w in reversed(e.walzen):
       #     c = w.walzenteil_rechts[c]
       #     c = w.walzenteil_links[c]
       # c = e.ukw[c]
       # for w in e.walzen:
       #     c = w.walzenteil_links[c]
      #      c = w.walzenteil_rechts[c]
      #  c = stecker[c]
      #  umgewandelter_text += chr(c+97)
   # return umgewandelter_text


#def umwandeln(e, text):
   # alphabet = np.arange(26)
   # stecker = np.array([e.stecker.get(i, i) for i in alphabet])
    #umgewandelter_text = ""
    # Umwandlung des Textes in NumPy-Array
    #text_array = np.array([ord(c)-97 if 0 <= ord(c)-97 <= 25 else -1 for c in text])
    # Filterung ungültiger Werte
    #valid_indices = np.where(text_array != -1)[0]
    #text_array = text_array[valid_indices]
    # Umwandlung
    #for i in range(len(text_array)):
       # e.rotieren()
       # text_array[i] = stecker[text_array[i]]
       # for w in reversed(e.walzen):
         #   text_array[i] = w.walzenteil_rechts[text_array[i]]
         #   text_array[i] = w.walzenteil_links[text_array[i]]
       # text_array[i] = e.ukw[text_array[i]]
       # for w in e.walzen:
        #    text_array[i] = w.walzenteil_links[text_array[i]]
          #  text_array[i] = w.walzenteil_rechts[text_array[i]]
       # text_array[i] = stecker[text_array[i]]
    # Rückumwandlung des NumPy-Arrays in einen String
    #umgewandelter_text = ''.join([chr(c+97) for c in text_array])
   # return umgewandelter_text



#enigma = Enigma_maschine()
#enigma.einstellungen((3, 1, 2), "cpt", 2, [2,3,4],  "") #die einstellung zum decodieren 

#enigma.einstellungen((1, 2, 3), "cpt", 2, [2,3,4],  "az") #die enstellung zum codieren 

#print('Enigma Simulator')
#text = input('Texteingabe:').lower()
#umgewandelter_text = umwandeln(enigma, text)
#print(umgewandelter_text)


#w = Walze(3, ord("t")-97, 4)
#w.prüfen()


#rotate ist eine default methode von deque, erspart das implementieren 
           



#standartwalze=["a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"]

#walzeI=["e","k","m","f","l","g","d","q","v","z","n","t","o","w","y","h","x","u","s","p","a","i","b","r","c","j"]
#walzeII=["a","j","d","k","s","i","r","u","x","b","l","h","w","t","m","c","q","g","z","n","p","y","f","v","o","e"]
#walzeIII=["b","d","f","h","j","l","c","p","r","t","x","v","z","n","y","e","i","w","g","a","k","m","u","s","q","o"]
#walzeIV=["e","s","o","v","p","z","j","a","y","q","u","i","r","h","x","l","n","f","t","g","k","d","c","m","w","b"]
#walzeV=["v","z","b","r","g","i","t","y","u","p","s","d","n","h","l","x","a","w","m","j","q","o","f","e","c","k"]

#walzenDict = {"I":walzeI,"II":walzeII,"III":walzeIII,"IV":walzeIV,"V":walzeV}


#ukw_A=["e","j","m","z","a","l","y","x","v","b","w","f","c","r","q","u","o","n","t","s","p","i","k","h","g","d"]
#ukw_B=["y","r","u","h","q","s","l","d","p","x","n","g","o","k","m","i","e","b","f","z","c","w","v","j","a","t"]
#ukw_C=["f","v","p","j","i","a","o","y","e","d","r","z","x","w","g","c","t","k","u","q","s","b","n","m","h","l"]


#uebertragskerbeI=["q"]
#uebertragskerbeII=["e"]
#uebertragskerbeIII=["v"]
#uebertragskerbeIV=["j"]
#uebertragskerbeV=["z"]

#uebertragskerbeDict={"I":uebertragskerbeI,"II":uebertragskerbeII,"III":uebertragskerbeIII,"IV":uebertragskerbeIV,"V":uebertragskerbeV}

