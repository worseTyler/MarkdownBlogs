
For the past several months, I have been participating with the DevOps Metrics team following the DevOps Forum 2015 earlier this year. The topic of discussion was Metrics related to DevOps and today I presented on the topic at the 2015 DevOps Enterprise Summit in San Francisco in a talk entitled Metrics that Matter.  The slides for this can be downloaded here:

**[Metrics that Matter slides (PDF)](/wp-content/uploads/2015/10/DevOps2015_Mark-Michaelis_Metrics-that-Matter.pdf)** 

During the presentation, I outlined the challenge of applying taxonomy to DevOps metrics. To begin with, consider an unorganized list of metrics:

<script src="http://cdn.tagul.com/embed/a2c5njinkl2a"></script>

[Created with Tagul.com](https://tagul.com/)

For those of you looking for a list you can cut and paste, here you go:

**Metrics List**

Alerting efficiency Attitude towards continuous improvement Average wait time per work item per release/time-period Budget adherence Change per release (work items/story points/lines of code) Cost impact per outage/down time Cost per release Cost to acquire new customer Defects per release/sprint/area Defects raised during UAT per release Deployment time (manual/automated) Down time per release Employee retention/turnover Employee satisfaction #experiments per release Frequency of customer tickets Frequency of failed deployments per release

Frequency of outages Frequency of release/change Frequency of undocumented changes #Individuals per skill (measuring cross-skilling) Lead time Mean time to repair/recover (MTTR) Outlier performance Ownership vs. blame Process time project/release/sprint Signal to noise ratio Similarity of production to test environments Skills per team/project System response time Team autonomy Technology experimentation Test code coverage and effectiveness Time outside SLA

Time per story point Time to communicate Time to complete automated tests Time to complete build Time to detect Time to escalate Time to implement per story point Time to integrate Time to mitigate Time to rollback Time to understand requirement per story point Total new/leaving customers per time Total time at company (retention) Unexpected expense per release Unplanned work per release Wait time per work item Work in progress per release/sprint

**However, we soon find that:**

- Metrics have different meanings depending on where they are being used.  Lead-time for a development team is different than lead-time for someone looking at Operations and different again from the perspective of the CXO.
- Whether a metric is relevant depends on your perspective.  Metrics that matter to development, build time or code coverage for example, are not so significant to those in operations or even senior management.
- Depending on the categorization, metrics frequently appear in more than one category and frequently span categories.

This results in a multitude of taxonomy options, pivoting across a number of dimensions:

\[caption id="attachment\_21161" align="aligncenter" width="600"\][![DevOps Metrics Taxonomy](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/10/devops-metrics/images/DevOpsMetricsTaxonomy-1024x494.png)](/wp-content/uploads/2015/10/DevOpsMetricsTaxonomy.png) Choosing a taxonomy for DevOps metrics is challenging.\[/caption\]

A core problem with metrics, is that their importance is relevant to your perspective.  What the CXO believes to be important is likely very different from what that nerd writing the code sees as important.  And, in fact, on occasion one metric that seems important, depends on the values of other metrics:

\[caption id="attachment\_21191" align="aligncenter" width="599"\][![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/10/devops-metrics/images/MetricsAreAMatterOfPerspective-1024x581.png)](/wp-content/uploads/2015/10/MetricsAreAMatterOfPerspective.png) Which metrics that matter are relative to the observer's perspective and even relative to other metrics.\[/caption\]

Ultimately, I find it useful to review all the possible metrics you can get your hands on and consider each one as a potential option, weighing the work required to gather the metric with the value that it provides.  In evaluating the possible metrics, consider whether a measurement is transparent, relevant, automated, comparative, and whether it will be examined:

[![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/10/devops-metrics/images/Principles-of-Measurement-1024x487.png)](/wp-content/uploads/2015/10/Principles-of-Measurement.png)
