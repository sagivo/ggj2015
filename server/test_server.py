
import requests

uid1 = 'asdf'
uid2 = 'blah'
base = 'http://floating-basin-3676.herokuapp.com/'
base = 'http://localhost:5000/'

r = requests.post(base+'lfg/'+uid1)
r = requests.post(base+'lfg/'+uid2)
game_id = r.content
r = requests.post(base+'games/'+game_id+'/sync/'+uid1)
print r.content
r = requests.post(base+'games/'+game_id+'/sync/'+uid2)
print r.content
r = requests.post(base+'games/'+game_id+'/sync/'+uid1)
print r.content
r = requests.post(base+'games/'+game_id+'/actions/'+uid1,data={'actions':'testactions1'})
r = requests.post(base+'games/'+game_id+'/actions/'+uid2,data={'actions':'testactions2'})
r = requests.get(base+'games/'+game_id+'/actions/'+uid1)
print r.content
r = requests.get(base+'games/'+game_id+'/actions/'+uid2)
print r.content
