
select *
from RutorListItems
order by AddedDate desc

select *
from TorrentSoftPosts

select *
from logs

update TorrentSoftPosts
set TorrentFile = ''
where Id = 2