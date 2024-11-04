# After running fetch_album_art.py, this script will update the AlbumArtUrl column in the album table

import pathlib
import json


if __name__ == '__main__':

    with open('album_lookup.json') as album_lookup:
        album_lookup = json.load(album_lookup)
    
    for album in album_lookup:

        # if image already exists, print update statement
        if pathlib.Path('..', 'images', '{}.jpg'.format(album['AlbumId'])).exists():
            print("UPDATE Albums SET AlbumArtUrl = '~/Images/Albums/{}.jpg' WHERE AlbumId = {};".format(album['AlbumId'], album['AlbumId']))
        

