

## Optimization of Power BI to Maximize Advantages of Encoding Methods
#
This guide explores data compression regarding Power BI, specifically relating to xVelocity engine functionality and data importation limitations.  In this discussion, I focus on **10 GB uncompressed data** and **1 GB compressed data**. 

### Limitations of 10 GB Uncompressed Data 

**Likely error encountered by** **our client**: “_The amount of uncompressed data on the gateway client has exceeded the limit of 10 GB for a single table. Please consider reducing the use of highly repetitive strings values through normalized keys, removing unused columns, or upgrading to Power BI Premium.”_ 

#### Question: How does the VertiPaq/xVelocity compression engine apply during the importation of data? 

**Answer**: Power BI service uses a defined limit of 10 GB uncompressed data per table allowed for a model without Premium, regardless of compression potential, with a post-compression limit of 1 GB. VertiPaq Compression does not take place until after the source dataset is read, meaning that there are alternative data-size reduction techniques that must be applied before data is imported into Power BI.  

**Description of** **VertiPaq****/****xVelocity** **engine**: 

The steps that happen during processing are as follows: 

1. Reading of the source dataset, transformation into a columnar data structure of VertiPaq, encoding and compressing each column. 

2. Creation of dictionaries and indexes for each column. 

3. Creation of the data structures for relationships. 

4. Computation and compression of all the calculated columns. 

Reference: [https://www.microsoftpressstore.com/articles/article.aspx?p=2449192&seqNum=1](https://www.microsoftpressstore.com/articles/article.aspx?p=2449192&seqNum=1) 

#### Question: How do I import a large dataset into Power BI if I am limited to 10 GB pre-compression? 

**Answer:**  As recommended by Microsoft, “If you're hitting this issue, there are good options to optimize and avoid it. In particular, reduce the use of highly constant, long string values and instead use a normalized key. Or, removing the column if it's not in use helps.” Since the limit is per table, importing multiple smaller tables instead of one large view may remedy this issue. Additionally, consider the efficiency of the source tables’ datatypes. Even a few excessive bit reservations can significantly increase the data footprint when multiplied by millions of rows. 

References:  

[https://docs.microsoft.com/en-us/power-bi/connect-data/service-gateway-onprem-tshoot](https://docs.microsoft.com/en-us/power-bi/connect-data/service-gateway-onprem-tshoot)  

[https://dzone.com/articles/practical-tips-to-reduce-sql-server-database-table](https://dzone.com/articles/practical-tips-to-reduce-sql-server-database-table)  

### Limitations of 1 GB Compressed Data 

#### Question: If my source is already less than 10 GB pre-compression how can I optimize my data to follow the 1 GB compressed limitation? 

**Answer**: If the dataset is small enough to be loaded into Power BI (under 10GB), then Power BI’s xVelocity In-Memory Analytics Engine (previously known as VertiPaq, which is how it is often referenced in documentation) further compresses the data into a columnar data structure. Column stores are efficient in reducing size and read time but may require more CPU usage. xVelocity uses three main methods for compression: value encoding, dictionary encoding, and run-length encoding. Understanding how compression functions can help us determine how to optimize the compressibility of our data. 

#### 1. Value encoding:

Only applicable to integer columns, this encoding method utilizes mathematical relationships within the data that can be leveraged to reduce data size. For example, in a column that contains only numerical values between 200 and 220, the VertiPaq engine might subtract 200 from each value, allowing it to only store values between 0 and 20, a reduction of 3 bits per value. These values must then be converted back when accessed. In general, an increase in CPU load is preferable to an increase in required reads, as it is cheaper to increase CPU speed than reduce memory access time. Value encoding is the most efficient encoding method, so integer columns should be used whenever possible. For example, you can apply value encoding by removing consistent text-value prefixes (e.g. “SO123456 being replaced with 123456 in a Sales Order table), or splitting decimal columns into two integer columns. 

References:  

[https://docs.microsoft.com/en-us/power-bi/guidance/import-modeling-data-reduction](https://docs.microsoft.com/en-us/power-bi/guidance/import-modeling-data-reduction)  

[https://www.fourmoo.com/2019/11/27/how-i-saved-40-on-my-power-bi-dataset-size/](https://www.fourmoo.com/2019/11/27/how-i-saved-40-on-my-power-bi-dataset-size/)  

[https://www.sqlbi.com/articles/optimizing-high-cardinality-columns-in-vertipaq/](https://www.sqlbi.com/articles/optimizing-high-cardinality-columns-in-vertipaq/)  

#### 2. Dictionary encoding:

Also known as hash encoding, this encoding method builds a dictionary of distinct values of a column and replaces column values with indexes to the dictionary. 

![Power BI dictionary encoding for data compression](https://intellitect.com/wp-content/uploads/2020/11/image.png "Power BI’s Data Compression: Large Data Imports in Power BI")

This process has two distinct advantages. First, because column values are replaced with integers, they are smaller to store and only require the minimum number of bits necessary to store an index entry. This can be seen in the example above which only requires 2 bits per entry to represent the four colors. Second, the replacement of columns with integers essentially makes VertiPaq datatype independent, so optimization of datatype is mostly relevant to the reduction of pre-compressed data size. The primary determinant of dictionary encoding efficacy is the cardinality of a column or the number of distinct values. Examples of cardinality reduction include splitting a DateTime column into separate Date and Time columns which reduces the number of unique values within a single column, removing “Notes” columns, or changing granularity from seconds to minutes. 

Reference:  

[https://exceleratorbi.com.au/solving-a-complex-time-problem-in-power-bi](https://exceleratorbi.com.au/solving-a-complex-time-problem-in-power-bi)  

#### 3.  Run Length Encoding: 

This encoding method pertains to columns with numerous repeating values. Instead of storing each instance of a repeated value, such as Q1 existing in the first 200 rows of a table, the xVelocity engine instead calculates and stores the number of times a value appears in a row, further compressing the data. 

![Data compression in Power BI](https://intellitect.com/wp-content/uploads/2020/12/RunLengthEncoding-300x266.png "Power BI’s Data Compression: Large Data Imports in Power BI")

This process is complementary to the prior encoding methods because run-length encoding can be applied to value-encoded data. Bear in mind, xVelocity will try to find the optimal column to sort by to maximize compression via run-length encoding and may spend up to 10 seconds per 1 million rows while determining the best column to use. Additionally, run-length encoding will likely result in the reordering of a table’s rows after loading. 

### Conclusion

Although Power BI’s data compression engine is remarkably efficient by default, as it often results in compression ratios of greater than 90% without altering data structures, it still leaves room for improvement by the end-user. In most of the projects where I’ve used Power BI, such as creating dashboards to help homeless shelters better track their success metrics, the default compression has been sufficient. However, in cases where datasets are larger than 1 GB additional steps are often necessary. 

What project will you use Power BI for next? Tell us in the comments below! 

### Want More?

For further information on optimizing Power BI, look to our other [blog posts](https://intellitect.com/blog/)!
