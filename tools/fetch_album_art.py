"""A simple script that searches for a release
"""

import musicbrainzngs
import json
import pathlib

musicbrainzngs.set_useragent(
    "python-musicbrainzngs-example",
    "0.1",
    "https://github.com/alastair/python-musicbrainzngs/",
)

if __name__ == '__main__':

    with open('album_lookup.json') as album_lookup:
        album_lookup = json.load(album_lookup)
    
    for album in album_lookup:

        # if image already exists, skip
        if pathlib.Path('..', 'images', '{}.jpg'.format(album['AlbumId'])).exists():
            print("Image already exists for {}".format(album['Title']))
            continue

        print("Searching for {} by {}".format(album['Name'], album['Title']))
        # The "search_*" functions take a variety of arguments to limit
        # the search. The "artist" and "release" arguments limit the
        # search to only those releases with the given artist and release
        # name. The "limit" keyword argument is special (like as "offset",
        # not shown here) and specifies the number of results to

        # Keyword arguments to the "search_*" functions limit keywords to
        # specific fields. The "limit" keyword argument is special (like as
        # "offset", not shown here) and specifies the number of results to
        # return.
        result = musicbrainzngs.search_releases(artist=album['Name'], release=album['Title'],
                                                limit=1)
        # On success, result is a dictionary with a single key:
        # "release-list", which is a list of dictionaries.
        if not result['release-list']:
            print("no release found", album['Name'], album['Title'])
        release = result['release-list'][0]

        release_id = release['id']
        try:
            data = musicbrainzngs.get_image_front(release_id, size="500")
            
            # write to images/
            with open('../images/{}.jpg'.format(album['AlbumId']), 'wb') as f:
                f.write(data)
        except:
            print("No front image found for {}".format(album['Title']))
