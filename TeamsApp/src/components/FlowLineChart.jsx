import React, { PureComponent } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class FlowLineChart extends PureComponent {
  render() {
    const { flows } = this.props;
    console.log(flows);

    // Define a list of colors for the lines
    const lineColors = [
      "#8884d8", // Purple/Blue
      "#82ca9d", // Light Green
      "#ffc658", // Orange/Yellow
      "#ff7300", // Orange
      "#d0ed57", // Lime Green
      "#a4de6c", // Light Lime
    ];

    return (
      <ResponsiveContainer width="100%" height="100%">
        <LineChart>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="time" type="category" allowDuplicatedCategory={false} />
          <YAxis 
            dataKey="statecode" 
            domain={[-0.1, 1.1]}
            ticks={[0, 1]} 
          />
          <Tooltip />
          <Legend />
          {flows.map((s, index) => (
            <Line
              key={s.name}
              dataKey="statecode"
              data={s.data}
              name={s.name}
              stroke={lineColors[index % lineColors.length]}
            />
          ))}
        </LineChart>
      </ResponsiveContainer>
    );
  }
}