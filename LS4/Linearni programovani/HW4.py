import re
from sys import stdin

mode, nodes, lines = input().split()
nodes = int(nodes)

table = [[0 for c in range(nodes)] for r in range(nodes)]
for i in range(len(table)):
	table[i][i] = 1

for line in stdin:
	table[int(line.split()[0])][int(line.split()[2])] = 1


print("""
# mnoziny
set I := 0..""",nodes-1,""";
set J := 0..""",nodes-1,""";

# promenne
var V{I,J}, >= 0, <= 1, integer;
var P{J}, >= 0, <= 1, integer;

# podminky
s.t. a{i in I}: sum{j in J} V[i,j] = 1;
s.t. b{i in I, j in J}: P[j] >= V[i,j];
""")

counter = 0

for row in range(len(table)):
	for column in range(row, len(table)):
		if(table[row][column] == 0):
			print("s.t. c_"+str(counter)+"{j in J}: V[",row,",j] + V[",column,",j] <= 1;")
			print("s.t. d_"+str(counter)+": V[",row,",",column,"] = 0;")
			print("s.t. e_"+str(counter)+": V[",column,",",row,"] = 0;")
			counter+=1


print("""
# min/max funkce
minimize obj: sum{j in J} P[j];

# vyres to!
solve;
""")

if(0==1):
	print("""
# napis debugovaci info
display obj;
display V;
printf {i in I, j in J} "%s%s%s%s", (if (V[i,j] = 1) then i else ""), (if (V[i,j] = 1) then ": " else ""), (if (V[i,j] = 1) then j else ""), (if (V[i,j] = 1) then ", " else "");
printf "\\n";
display P;
	""")

print("""
# napis vysledek
printf "result: %d\\n", obj.val;

# umri
end;
""")
