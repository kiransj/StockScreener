#!/bin/bash 

for i in `seq 01 31` ;
do
    filename=$(printf "cm%02dNOV2017bhav.csv.zip" $i)
    url="https://www.nseindia.com/content/historical/EQUITIES/2017/NOV/$filename"
    echo "Downloading $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    echo $SIZE
    if [ $SIZE -lt 1000 ] 
    then
        echo "Deleteing file $filename"
        rm $filename
    else
        unzip $filename
        rm $filename
    fi
done
