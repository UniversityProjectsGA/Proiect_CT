import re

token_regex = [
    (r'\/\/.*', 'comment'),
    (r'/\\*.*?\\*/', 'comment'),
    (r'\/\*.*', 'comment start'),
    (r'.*\*\/', 'comment end'),
    (r'\b(auto|break|case|char|const|continue|default|do|double|else|enum|extern|float|for|goto|if|int|long|main|register|return|short|signed|sizeof|static|struct|switch|typedef|union|unsigned|void|volatile|while)\b', 'key_word'),
    (r'.*?\"(.*)\".*', 'string_constant'),
    (r'.*?\'(.*)\'.*', 'character_constant'),
    (r'[a-zA-Z_][a-zA-Z0-9_]*', 'identifier'),
    (r'\d*\.\d+|\d+\.\d*', 'float_constant'),
    (r'\d+', 'integer_constant'),
    (r'\+\+|\-|\*|\/|\%|\+|\-\-|\=\=|\!\=|\>|\<|\>\=|\<\=|\&\&|\|\||\!|\<\<|\>\>|\~|\&|\*|\^|\||\=|\+\=|\-\=|\*\=|\/\=|\%\=', 'operator'),
    (r'\(|\)|\[|\]|\{|\}|\;|\,|\:', 'delimitator')
]

def analizare_lexicala(linie, numar_linie):
    tokens = []
    for regex, tip_token in token_regex:
        t_gasit = re.findall(regex, linie)
        for tg in t_gasit:
            lungime = len(tg)
            tokens.append((tg, tip_token, lungime, numar_linie))
        linie = re.sub(regex, '', linie)
    if linie.strip():
        print(f"token necunoscut pe linia {numar_linie}: {linie.strip()}")
    return tokens

def citire_fisier(fisier):
    with open(fisier, 'r') as f:
        rez = []
        numar_linie = 1
        for linie in f:
            rez.extend(analizare_lexicala(linie.strip(), numar_linie))
            numar_linie += 1
        return rez

rez = citire_fisier('test.txt')
for token in rez:
    print(f"{token[0]}, {token[1]}; {token[2]}; linia {token[3]}")