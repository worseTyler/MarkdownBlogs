

### Need to scale large applications? Learn why Kubernetes is a great Docker add-on.

Creating an application is a big deal, but that’s not the only step in the development process. Often people forget about the sometimes difficult act of deploying their product to the end-user.

Application deployment has changed over time. And although we have several popular cloud deployment solutions, there are challenges that come with deploying products. Balancing cost and performance is one of these challenges.

Let’s look at the fundamentals of containers and the need for container orchestration solutions for large projects. We’ll discover how Docker and Kubernetes provide simplify the development process. We’ll also learn what makes them different and figure out the best situations to use each of them.

#### **History of deployment**

Traditionally, all applications ran on physical servers. But there were limitations. Server resources, for example, couldn’t be set aside for specific applications. So virtualized deployment was introduced.

Each server hosts multiple virtual machines (VM), each of which can be configured with specific resource allocations preventing the shortcoming of traditional deployment. In addition to improved resource management, VMs also add a layer of security because applications cannot freely access each other.

[Containers](https://www.docker.com/resources/what-container) further improved the deployment cycle. They work very similarly to VMs but are made more lightweight by eliminating the operating system. Each container still has its separate file system and resource allotment, but because it is no longer coupled to an operating system, they are portable, can be used across environments, and used more easily in the Cloud.

#### **Enter Docker**

Let’s look at possibly one of the most widely used products to come from containerized deployment, Docker. [Click here](/docker-scaffold/) for an introduction to Docker. Docker is a containerization technology provided as a set of coupled software as a service (SaaS) and platform as a service (PaaS). Since its release in 2013, many companies have adopted its technologies like IBM, GE appliances and Lyft, to name a few.

I’m sure at this point you’re thinking, “I came here for Kubernetes, why are you telling me how great Docker is?” I believe it’s essential to first understand some of the details surrounding containers and how they are constructed and maintained. So what better product to look at than one of the most popular?

#### Docker works in containers

Let’s say you are developing an application using Javascript. Your development machine is running Windows 10, The application is written for Ubuntu 16.04, and your deployment server runs Ubuntu 5.04. You need to keep track of a lot of variables! Docker provides a way to alleviate the stress of cross-platform developing by allowing your application to be developed and run in containers.

Here’s how it works. You start by creating a Docker image that is essentially a read-only template that houses your code, libraries, and any dependencies your application needs to run. You give that image to Docker, which automatically creates containers using that image as a template.

Now that you have a container, you can test and develop your application on your local machine. When it’s time to put your application on a development server or release it into production, you’ll know your application will behave exactly the same regardless of host operating system because each container created from the same Docker image is a carbon copy of one another.

#### Kubernetes in a nutshell

Say the development process using Docker worked well and your product is starting to take off. Your container is getting all the traffic it can handle, and it’s time to expand. You add another container. Soon you need another, and so on.

Each time you have to create a new container manually, you risk performance issues due to limiting resources. That would quickly become a tiresome process! However, there’s  a simpler way to manage your containers without manually intervening every time there is a rush of users.

Kubernetes (k8s) can solve all your container management woes. Kubernetes is an open-source system that used to deploy, scale and manage containerized applications. That means it’s a container orchestration system that can automatically manage your containers. It takes configuration (desired state) from the user and continuously works toward that state. The whole system, usually called a cluster, becomes apparent when we look at Kubernetes’ two fundamental components.

#### Kubernetes' fundamental components

**Nodes**: A node is either a VM or a physical machine that contains all the necessary services to run application containers. It achieves this by leveraging Docker or another container engine to create the containers on each node. Nodes also provide a Kubelet” to manage containers and a “Kube Proxy” for end-users to access the application.

**Control Plane**: The control plane, often called a primary, is the central controlling entity for the nodes. It manages the configuration for all the nodes and continuously works to achieve the desired state. Any persistent configuration data exists in the control plane in what’s called the “etcd.” It’s essentially a database that holds key-value pairs representing the desired state set by the deployer. The Kubernetes API also resides in the control plane and allows users to make changes to their configuration. The API is most commonly accessed through a command-line syntax called “Kubectl.”

 "Discover Kubernetes: a popular container orchestration solution"

Kubernetes architecture overview graphic from [Andrew Martin's blog](https://kubernetes.io/blog/2018/07/18/11-ways-not-to-get-hacked/) on kubernetes.io.

Together, a set of nodes that hold your application and the control plane make up what we call Kubernetes.

#### Kubernetes vs. Docker

Docker is a popular container technology, whereas Kubernetes is a popular container _orchestration_ solution. So, the question isn’t “should you use Docker over Kubernetes” or vice versa. The question is, when should you use each product?

Docker is used to create containers on a single host. On the other hand, Kubernetes _manages_ container technologies like Docker on multiple hosts, and it MUST have some third party container engine for container creation. From there, it works to keep the overall state of all the container healthy. By delegating the functionality of container creation to another service, Kubernetes can focus on scaling and creating multiple instances of Docker, and each instance of the container engine can focus specifically on the containers for one of the many Kubernetes nodes.

Even though Docker and Kubernetes are different solutions, to different problems, they integrate well. Docker comes with two container orchestration solutions – Kubernetes and Docker Swarm (Docker’s form of container orchestration). Docker saw the rising popularity of Kubernetes and chose to directly integrate it rather than compete with it. The fact that Docker includes both orchestration systems out of the box speaks volumes about Kubernetes’ reputation.

#### When to use Kubernetes

Kubernetes is not for everyone. It’s only needed for scaling large applications. That’s not to say small projects cannot use it, but it doesn’t make sense to set Kubernetes up if there’s only going to be one container ever used. For example, a personal, non-commercial website, may not get much traffic, so using a Docker container would be sufficient. Even in that case, if you are writing an application that has no cross-platform, scaling, or portability requirements, then you probably don’t even need Docker. In that case, hosting your application in a virtual machine or on a traditional host would work sufficiently without adding unnecessary complexity.

Container deployment has its benefits for smaller endeavors, but in large-scale deployments, one container won’t cut it. While traditional and virtual deployments were (and many times, still are) very effective in multiple situations, Kubernetes improves on one of the most significant deciding factors people have. Cost.

Need to handle X number of concurrent requests to your website when your product first launches? You could buy the machines that can handle that, but that takes a high initial investment, and users won’t utilize most of those resources after the initial rush of traffic.

If you use Kubernetes, you could automatically scale your number of nodes, i.e., machines in use, when they aren’t needed.

So, if you were using Kubernetes as SaaS, and you were hosting the containers on machines from a third party (like Azure), you could easily decrease financial output when needed. The moment traffic picks up, Kubernetes can create the necessary containers to achieve a near-perfect balance of performance to cost. That’s something traditional deployment cannot do.

#### Conclusion

Although Kubernetes is not a fix-all for deployment challenges, it can certainly save time and money in the right situations. It’s a powerful tool for cloud services or enterprise-level software. Despite the numerous container orchestration solutions available, Kubernetes seems to be becoming more and more popular every day. It’s not just the base Kubernetes software that is used frequently. Many sub-products have been created to augment Kubernetes.

If you are looking to set up a scalable and maintainable container solution, or think you may need it in the future, Kubernetes should be your first choice for container orchestration.

Don’t take my word for it; go try it out!

Kubernetes is open-source; that means many services integrate it to make your life easier. You can use many of the big players like AWS, Google Cloud Services, and Azure, to name a few. But why pay for hosting services if you don’t need to? You can run it on your own existing machines. Likewise, several services can even manage your cluster, giving you time to work on what matters – the product!

Is Kubernetes your go-to container orchestration tool? Let us know in the comments below.
