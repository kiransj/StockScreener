#!/bin/bash 

for i in `seq 01 31` ;
do
    filename=$(printf "MTO_%02d112017.DAT" $i)
    url="https://www.nseindia.com/archives/equities/mto/$filename"
    echo "Downloading $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    echo $SIZE
    if [ $SIZE -lt 1000 ] 
    then
        echo "Deleteing file $filename"
        rm $filename
    fi
done
