import axios from 'axios';
import React from 'react';
import Cookies from 'universal-cookie';
import {Bar} from 'react-chartjs-2';

import './DashBoard.css';

class DashBoard extends React.Component {
    state = {
        tickets: []
    }

    _isMounted = false;

    async componentDidMount() {
        this._isMounted = true;
        const cookies = new Cookies();

        const projectsPromise = await axios.get('https://localhost:5000/api/projects?email=' + cookies.get('email'));
        const projects = await projectsPromise.data;
        const promises = [];

        for (const project of projects) {
            promises.push(axios.get(`https://localhost:5000/api/projects/${project.projectGUID}/tickets?email=${cookies.get('email')}`));
        }

        if (this._isMounted) {
            Promise.all(promises)
                .then(values => {
                    let mergedValues = [];
                    for (const value of values) {
                        mergedValues = mergedValues.concat(value.data);
                    }

                    if (this._isMounted) {
                        console.log("set state");
                        console.log(mergedValues);
                        this.setState({tickets: mergedValues});
                    }
                    else {
                        console.log("unmounted already");
                    }
                });
        }
    }

    componentWillUnmount() {
        this._isMounted = false;
    }

    render() {
        console.log(this.state.tickets);

        const cookies = new Cookies();

        const data = {
            labels: ['Low', 'Medium', 'High'],
            datasets: [
                {
                    label: 'Tickets',
                    data: [
                        this.state.tickets.filter(ticket => ticket.ticketPriority === 0).length,
                        this.state.tickets.filter(ticket => ticket.ticketPriority === 1).length,
                        this.state.tickets.filter(ticket => ticket.ticketPriority === 2).length
                    ],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(255, 99, 132, 0.2)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }
            ]
        }

        const options = {
            scales: {
              yAxes: [
                {
                  ticks: {
                    min: 0,
                    stepSize: 1
                  },
                },
              ],
            },
          };

        return (
            <div>
                <div className="ticket-priority-chart-container">
                    <Bar data={data} options={options} />
                </div>
            </div>
        );
    }
}

export default DashBoard;