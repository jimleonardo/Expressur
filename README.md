# Expressur

## Grammar

*identifier* := [A-Za-z_][A-Za-z_0-9*]

*number* := ^-?[0-9]\d*(\.\d+)?$

*token* := *identifier* | *number*

*operator* := [*/+-]

*expression* := [(]*expression*|*token* *operator* *expression*|*token*[)]
