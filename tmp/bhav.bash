#!/bin/bash 

for i in `seq 8 8` ;
do
    #download stock prices
    filename=$(printf "cm%02dDEC2017bhav.csv.zip" $i)
    url="https://www.nseindia.com/content/historical/EQUITIES/2017/DEC/$filename"
    echo "Downloading Stock data from $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    if [ $SIZE -gt 1000 ] 
    then
        unzip $filename
    fi
    rm $filename

    #download deliverables information
    filename=$(printf "MTO_%02d122017.DAT" $i)
    url="https://www.nseindia.com/archives/equities/mto/$filename"
    echo "Downloading Deliverables information from $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    if [ $SIZE -lt 1000 ] 
    then
        echo "Deleteing file $filename"
        rm $filename
    else
       sed -i 1,3d $filename 
    fi
done
