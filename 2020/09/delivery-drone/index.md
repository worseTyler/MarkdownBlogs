

https://www.youtube.com/watch?v=IsRW7IdYGq8

## How I Constructed a Plane to Deliver Snacks
#
A few years ago, I saw an article about [Zipline](https://flyzipline.com/), a medical supply distribution company. They use an ingeniously designed plane system to deliver blood and other vital medical supplies to remote hospitals in Uganda. I thought that was the coolest use of RC aircraft drone I had seen! Indeed, their namesake “zippy,” a catapult launching system and super smart “clothes-line” aircraft arresting system, is quite the feat of automation. I thought it would be sweet to recreate their system, after all, software engineers never have enough side projects, do we? In all seriousness, passionate side projects are one of my favorite parts of the software engineering community.

#### Creating a Delivery Drone

![Launching the Drone](https://intellitect.com/wp-content/uploads/2020/09/Austen-Drone.jpg "VPNs Cliff Bars and a Delivery Drone")

Preparing to launch the drone for a test flight.

First, I wanted to start with the delivery plane part. Like the philanthropic mission of Zip line, I too envisioned a noble cause for my delivery plane: I thought it would be sweet to be out running or biking and have a drone deliver me a Cliff Bar.

#### Slashing the Budget

A delivery plane needs range. There are tons of RC radio systems available to control planes at a distance, but quality gear is expensive. I thought that in this DIY age, there must be a way to do this inexpensively.

I began poking around online, and a few forums later, I learned that yes, with a Raspberry Pi a companion flight controller hat, and a mobile SIM-based modem, I would be able to fly wherever a cell tower was in range.

This would be an RC plane unlike anything I could have imagined as a kid!

I purchased the French-made Navio2 flight controller hat for the Raspberry Pi 3 B+ for around $200 at the time, which is well under the cost of a good FPV and telemetry System being that I would be using my laptop for both.

I also bought the cheapest plane on Hobby King that would meet weight and space requirements – the reliable and easy to fly Bixler 3.

#### Creating the FPV and Telemetry Network

I purchased two Sim cards for an inexpensive mobile virtual network operator (MVNO) network. I used one in my phone that would be the modem for the laptop ground station (no need to buy a separate modem) and one in a Huawei 4G modem that plugged into the Raspberry Pi.

The real trick was figuring out how to get a direct connection between these two devices.

I entered the rabbit hole of network address translation (NAT) but found a relatively simple and elegant solution to the mess of the NAT world: [ZeroTier One](https://github.com/zerotier/ZeroTierOne).

##### ZeroTier One Saves the Day

ZeroTier One is a service that allows for the creation of a virtual private network (VPN) across most types of NATs. It uses an IPv4 network behavior hack called UDP hole punching. Feel free to read about that [elsewhere](https://www.zerotier.com/2014/08/25/the-state-of-nat-traversal/), in more detail and accuracy, but here’s how I utilized ZeroTier One for my situation.

![Prepping for launch](https://intellitect.com/wp-content/uploads/2020/09/Controls-1-1024x827.jpg "VPNs Cliff Bars and a Delivery Drone")

Preparing my drone and control system for preflight checks.

My Pi flight controller and laptop both sit behind a NAT with anonymous and “randomly” assigned IP’s and have no way of sending a message to each other. ZeroTier One facilitates creating a type of tunnel between them by giving them a communal place to meet.

UDP Packets are sent from both devices to a ZeroTier One server, which can use the source info from one device’s packets to address the destination of the other device’s packets. In simpler terms, it receives two letters with only return addresses on the envelopes. It can then fill in the “to” address with the return address of the other message. Quickly, a bridge is made as the address tables in the NAT router’s start seeing these packets.

Eventually, a direct connection is possible between the two devices, and the packets no longer go through the middleman, ZeroTier One server.

In my case, this results in a remote shell in the cloud(s). There was no need to buy a public IP service or configure some sort of port forwarding.

I use two modems on the same cellular network to decrease latency. It would also work between different ISP’s.

#### Testing, Testing, and More Testing

After months of reading, ordering parts, software configuring, and plane assembling, my dreams of remote, on-demand Cliff Bars were coming true!

On the first day of mild weather, I taxied my meticulously programmed aircraft out of its living room hangar, launched it into the air, and watched it fly.

Moments later, it promptly hurled into the ground.

In fact, I crashed the plane twice over the next few months (ordering replacement parts then repairing it in between tests).

![Plane and rockets](https://intellitect.com/wp-content/uploads/2020/09/Drone-Flying-1024x576.jpg "VPNs Cliff Bars and a Delivery Drone")

Celebrating Fourth of July with some Estes model rocket "missiles"!

I was so frustrated with my lack of ability to get it to fly, but it was an introspective learning experience for me. It wasn’t a lack of knowledge that caused me to fail; it was my approach to the activity.

I was too excited and skipped simple preflight checks that could have meant success. At one point, the channels were reversed on my transmitter and not in the autopilot settings. So, I could fly the plane manually, but the autopilot system’s aileron direction was inverted. The trial and error, although frustrating, left me with this much-needed learning experience that I can apply to all aspects of software development.

So, in the spirit of the DIY pioneers that I learned from over this year, here are my mistakes and how you can avoid them.

#### Preflight Checks!

Good pilots do them. (I would not fly with even Chuck Yeager if I knew he had not inspected his plane).

My biggest take away from the project: make a list of what should be checked and commit yourself to reviewing it. When you get excited about something, you will forget basic steps without a plan and a routine. That’s human nature. For me, I nulled hours of preparation and reading by forgoing a few minutes of checks.

Know it or not, everything we do has preflight checks. Whether we follow them or have the right list defines our results.

Starting a relationship - preflight checks.

Buying a home - preflight checks.

These preflight checks aren’t always easy or fast, but they’re worth it. So, commit ahead of time to do them.

#### Simulation!

Most aviation accidents occur when a pilot encounters a new variable that they haven’t experienced before.

When you have the chance to practice in advance, do it! After my first failures, I started looking for ways to do this.

[Mission Planner](https://ardupilot.org/planner/docs/mission-planner-overview.html) (my favorite [open-source ground control platform](https://github.com/ArduPilot/MissionPlanner) maintained by Micheal Osborne and written in C#) has a simulation feature.

Much like an integration test, simulation tests allow you to ensure that you have programmed a mission correctly and that you’ve mapped your joystick controls accurately. This tool has saved me from some possible mishaps.

If you can rehearse anything in life, it is worth the time.

### The Current Status of the Project

My plane now has flown many successful “missions.” It delivered a chocolate bar to my mom on her birthday while she was kayaking in the middle of a lake.

The 4th of July was spent entertaining my little cousins by dropping army men all day, even into the night.

I’ve even started dropping candies with little parachutes on the neighborhood kids- the year of development was worth the smiles.

The next phase of the project is to build an automated launch system that I can initiate remotely, bringing me one step closer to being able to go for a run and then request a Cliff Bar!

### Your Turn

In the comments, feel free to ask questions or provide suggestions about my drone experiment.

What is your most recent passion project?
